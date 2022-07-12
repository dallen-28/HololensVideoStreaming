using UnityEngine;
using System;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
//using System.Runtime.InteropServices;

public class OpenIGTLinkConnect : MonoBehaviour
{

    public int scaleMultiplier = 1000; // Metres to millimetres

    //Set from config.txt, which is located in the project folder when run from the editor
    public string ipString = "127.0.0.1";
    public int port = 18944;
    public GameObject[] GameObjects;
    public GameObject virtualDisplay;
    private int width;
    private int height;
    private int packetSize;
    public int msDelay = 33;

    private int maxBytes = 0;

    private float totalTime = 0f;

    //CRC ECMA-182
    private CRC64 crcGenerator;
    private string CRC;
    //private string crcPolynomialBinary = "10100001011110000111000011110101110101001111010100011011010010011";
    private string crcPolynomialBinary = "1000000011100000111000011110101110101001111010100011011010010011";
    private ulong crcPolynomial;

    private Socket socket;
    private IPEndPoint remoteEP;

    // ManualResetEvent instances signal completion.
    private static ManualResetEvent sendDone = new ManualResetEvent(false);

    // Receive transform queue
    public readonly static Queue<Action> ReceiveTransformQueue = new Queue<Action>();

    // Receive image queue
    public readonly static Queue<Action> ReceiveVideoQueue = new Queue<Action>();

    // Frame data queue
    public static Queue<byte[]> byteQueue = new Queue<byte[]>();

    private bool connectionStarted = false;

    private MeshRenderer virtualDisplayRenderer;

    // Use this for initialization
    void Start()
    {
        virtualDisplayRenderer = this.virtualDisplay.GetComponent<MeshRenderer>();

        // Uses editor values if none set in config file
        width = virtualDisplay.GetComponent<ScreenResolution>().width;
        height = virtualDisplay.GetComponent<ScreenResolution>().height;

        // Initialize CRC Generator
        crcGenerator = new CRC64();

        try
        {
            crcPolynomial = Convert.ToUInt64(crcPolynomialBinary, 2);
        }
        catch (Exception e)
        {
            Debug.Log("Test");
            Debug.Log(crcPolynomial.ToString());
            Debug.Log(String.Format("Exception : {0}", e.ToString()));
        }

        crcGenerator.Init(crcPolynomial);

        try
        {
            // Load settings from file
            // Format of Config file is:
            // IPAddress
            // Port
            // Image Width
            // Image Height

            string path = "config.txt";

            // Uncomment line below for deploying to Hololens
            //string path = Path.Combine(Application.persistentDataPath, "config.txt");


            //FileInfo iniFile = new FileInfo("config.txt");
            FileInfo iniFile = new FileInfo(path);
            StreamReader reader = iniFile.OpenText();
            string ipStringText = reader.ReadLine();
            string portText = reader.ReadLine();
            string widthText = reader.ReadLine();
            string heightText = reader.ReadLine();

            if (ipStringText != string.Empty && ipStringText != null)
                ipString = ipStringText;
            if (portText != string.Empty && portText != null)
                port = int.Parse(portText);
            if (widthText != string.Empty && widthText != null)
                width = int.Parse(widthText);
            if (heightText != string.Empty && heightText != null)
                height = int.Parse(heightText);

            virtualDisplay.GetComponent<ScreenResolution>().UpdateAspectRatio(width, height);
        }
        catch(Exception e)
        {
            Debug.Log(String.Format("Exception : {0}", e.ToString()));
        }
    
        
        // TODO: Connect on prompt rather than application start
        //StartupClient();
    }
    public void OnConnect()
    { 
        StartupClient();
    }

    private void StartupClient()
    {
        // Attempt to Connect
        try
        {
            // Establish the remote endpoint for the socket.
            IPAddress ipAddress = IPAddress.Parse(ipString);
            remoteEP = new IPEndPoint(ipAddress, port);

            // Create a TCP/IP  socket.
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Blocking = false;

            try
            {
                // Connect the socket to the remote endpoint. Catch any errors.
                socket.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), socket);
                connectionStarted = true;

                Receive(socket);
                Debug.Log(String.Format("Ready to receive data"));
            }
            catch (Exception e)
            {
                Debug.Log(String.Format("Exception : {0}", e.ToString()));
            }
        }
        catch (Exception e)
        {
            Debug.Log(String.Format(e.ToString()));
        }
    }

    private void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.
            Socket client = (Socket)ar.AsyncState;

            // Complete the connection.
            client.EndConnect(ar);

            Debug.Log(String.Format("Socket connected"));

            // Signal that the connection has been made.
            //connectDone.Set();
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Repeat every msDelay millisecond
        //if (totalTime * 1000 > msDelay)
        //{
            if (connectionStarted)
            {
                // Send Transform Data if Flag is on
                foreach (GameObject gameObject in GameObjects)
                {
                    if (gameObject.GetComponent<OpenIGTLinkFlag>().SendTransform)
                    {
                        SendTransformMessage(gameObject.transform);
                    }

                    if (gameObject.GetComponent<OpenIGTLinkFlag>().SendPoint & gameObject.GetComponent<OpenIGTLinkFlag>().GetMovedPosition())
                    {
                        SendPointMessage(gameObject.transform);
                    }
                }
                // Perform all queued Receive Transforms and Images
                while (ReceiveTransformQueue.Count > 0)
                {
                    ReceiveTransformQueue.Dequeue().Invoke();
                }
                while (ReceiveVideoQueue.Count > 0)
                {
                    ReceiveVideoQueue.Dequeue().Invoke();
                }
            }
            // Reset timer
            //totalTime = 0f;
        //}
        //totalTime = totalTime + Time.deltaTime;
    }

    void OnApplicationQuit()
    {
        // Release the socket.
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
    }

    // -------------------- Receive -------------------- 
    private void Receive(Socket client)
    {
        try
        {
            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = client;
            //Debug.Log("Created New State Object with Thread: " + Thread.CurrentThread.ManagedThreadId.ToString());

            // Begin receiving the data from the remote device.
            ReceiveStart(state);
        }
        catch (Exception e)
        {
            Debug.Log(String.Format(e.ToString()));
        }
    }

    private void ReceiveStart(StateObject state)
    {
        //Debug.Log("Receive Start called with Thread: " + Thread.CurrentThread.ManagedThreadId.ToString());
        Socket client = state.workSocket;
        client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the state object and the client socket 
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket client = state.workSocket;

            // Read data from the remote device.
            int bytesRead = client.EndReceive(ar);
            Debug.Log("Bytes Read: " + bytesRead);

            // As far as I can tell, Unity will not let the callback occur with 0 bytes to read, so I cannot use a 0 bytes left method to determine ending, must read the data type and size from the Header
            // TODO: Current workaround: adding check for a full buffer of transforms (divisible by 106), this may fail with other data types, must make overflow buffer work as well
            // Update: Overflow buffer fix: 
            // Recursively call client BeginReceive from callback function to avoid race condition caused from BeginReceive being called in main update thread
            // Could reinvestigate using manualresetevent to potentially increase performance but most likely server side would need to be optimized
            if (bytesRead > 0)
            {

                // There might be more data, so store the data received so far.
                byte[] readBytes = new Byte[bytesRead];
                Array.Copy(state.buffer, readBytes, bytesRead);
                state.byteList.AddRange(readBytes);
                bool moreToRead = true;

                state.totalBytesRead += bytesRead;
                Debug.Log("Total Bytes Read: " + state.totalBytesRead);

                while (moreToRead)
                {
                    // Read the header and determine data type
                    if (!state.headerRead && state.byteList.Count > 0)
                    {
                        string dataType = Encoding.ASCII.GetString(state.byteList.GetRange(2, 12).ToArray()).Replace("\0", string.Empty);
                        state.name = Encoding.ASCII.GetString(state.byteList.GetRange(14, 20).ToArray()).Replace("\0", string.Empty);
                        byte[] dataSizeBytes = state.byteList.GetRange(42, 8).ToArray();

                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse(dataSizeBytes);
                        }
                        state.dataSize = BitConverter.ToInt32(dataSizeBytes, 0) + 58;

                        Debug.Log(String.Format("Data is of type {0} with name {1} and size {2} with thread {3}", dataType, state.name, state.dataSize, Thread.CurrentThread.ManagedThreadId.ToString()));

                        if (dataType.Equals("IMAGE"))
                        {
                            state.dataType = StateObject.DataTypes.IMAGE;
                        }
                        else if (dataType.Equals("TRANSFORM"))
                        {
                            state.dataType = StateObject.DataTypes.TRANSFORM;
                        }
                        else if (dataType.Equals("STATUS"))
                        {
                            Debug.Log("Status");
                            state.dataType = StateObject.DataTypes.STATUS;
                        }
                        else
                        {
                            throw new Exception("Data is out of sync, please reconnect");
                        }
                        state.headerRead = true;

                    }

                    else if (state.totalBytesRead == state.dataSize)
                    {
                        Debug.Log("Exact Amount of data");

                        // All the data has arrived; put it in response.
                        if (state.byteList.Count > 1)
                        {
                            // Send off to interpret data based on data type
                            if (state.dataType == StateObject.DataTypes.TRANSFORM)
                            {
                                ReceiveTransformQueue.Enqueue(() =>
                                {
                                    StartCoroutine(ReceiveTransformMessage(state.byteList.ToArray(), state.name));
                                });
                            }
                            else if (state.dataType == StateObject.DataTypes.IMAGE)
                            {
                                // Create parallel image data queue 
                                byteQueue.Enqueue(state.byteList.ToArray());
            
                                ReceiveVideoQueue.Enqueue(() =>
                                {
                                    // Previous CoRoutine call would sometimes fail as no copy of the state data was made
                                    // Fix: Create image data queue to keep copy of the image corresponding to action queue
                                    StartCoroutine(ReceiveVideoMessage(byteQueue.Dequeue(), state.name));
                                });
                            }
                            // Status handling
                        }
                        // Signal that all bytes have been received.
                        moreToRead = false;
                        Receive(client);
                    }
                    else if ((state.totalBytesRead > state.dataSize) && (state.byteList.Count > state.dataSize) && (state.dataSize > 0))
                    {
                        Debug.Log("Overflow");

                        // More data than expected has arrived; put it in response and repeat.
                        // Send off to interpret data based on data type
                        if (state.dataType == StateObject.DataTypes.TRANSFORM)
                        {
                            ReceiveTransformQueue.Enqueue(() =>
                            {
                                try
                                {
                                    StartCoroutine(ReceiveTransformMessage(state.byteList.GetRange(0, state.dataSize).ToArray(), state.name));
                                }
                                catch (Exception e)
                                {
                                    Debug.Log(String.Format("{0} receiving {1} with total {2}", state.byteList.Count, state.dataSize, state.totalBytesRead));
                                    Debug.Log(String.Format(e.ToString()));
                                }
                            });
                        }
                        else if (state.dataType == StateObject.DataTypes.IMAGE)
                        {       
                            // Enqueue parallel image queue 
                            byteQueue.Enqueue(state.byteList.GetRange(0, state.dataSize).ToArray());

                            ReceiveVideoQueue.Enqueue(() =>
                            {
                                try
                                {                                   
                                    StartCoroutine(ReceiveVideoMessage(byteQueue.Dequeue(), state.name));
                                }
                                catch (Exception e)
                                {
                                    Debug.Log(String.Format("{0} receiving {1} with total {2}", state.byteList.Count, state.dataSize, state.totalBytesRead));
                                    Debug.Log(String.Format(e.ToString()));
                                }
                            });
                        }
                        state.byteList.RemoveRange(0, state.dataSize);
                        state.totalBytesRead = state.totalBytesRead - state.dataSize;
                        state.dataSize = 0;
                        state.name = "";
                        state.headerRead = false;
 
                    }
                    else
                    {
                        moreToRead = false;
                        // Get the rest of the data.
                        ReceiveStart(state);
                    }
                }
            }
            else
            {
                Debug.Log("Bytes less than zero");
            }
        }
        catch (Exception e)
        {
            Debug.Log("Overflow Bug");
            Debug.Log(string.Format(e.ToString()));
            // Release the socket.
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();

            // Attempt to Restart
            StartupClient();
        }
    }

    IEnumerator ReceiveVideoMessage(byte[] data, string imageName)
    {

        //Debug.Log("Receive Video");
    
        yield return null;

        // Plus MMF Video config file - Image size 640 x 480
        // Packet size of 921730 bytes = 130 bytes header + 921600 (640 * 480 * 3 (1 byte for R, 1 for G, 1 for B)) bytes image

        var tex = new Texture2D(this.width, this.height, TextureFormat.RGB24, false);
        tex.SetPixelData(data, 0, 130);

        // Doesn't work if set to true
        tex.Apply(updateMipmaps: false);
        this.virtualDisplayRenderer.material.mainTexture = tex;

    }

    IEnumerator ReceiveTransformMessage(byte[] data, string transformName)
    {
        // Find Game Objects with Transform Name and determine if they should be updated
        string objectName;
        foreach (GameObject gameObject in GameObjects)
        {
            // Could be a bit more efficient
            if (gameObject.name.Length > 20)
            {
                objectName = gameObject.name.Substring(0, 20);
            }
            else
            {
                objectName = gameObject.name;
            }

            if (objectName.Equals(transformName) & gameObject.GetComponent<OpenIGTLinkFlag>().ReceiveTransform)
            {
                // Transform Matrix starts from byte 58 until 106
                // Extract transform matrix
                byte[] matrixBytes = new byte[4];
                float[] m = new float[12];
                for (int i = 0; i < 12; i++)
                {
                    Buffer.BlockCopy(data, 58 + i * 4, matrixBytes, 0, 4);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(matrixBytes);
                    }

                    m[i] = BitConverter.ToSingle(matrixBytes, 0);

                }

                // Slicer units are in millimeters, Unity is in meters, so convert accordingly
                // Definition for Matrix4x4 is extended from SteamVR
                Matrix4x4 matrix = new Matrix4x4();
                matrix.SetRow(0, new Vector4(m[0], m[3], m[6], m[9] / scaleMultiplier));
                matrix.SetRow(1, new Vector4(m[1], m[4], m[7], m[10] / scaleMultiplier));
                matrix.SetRow(2, new Vector4(m[2], m[5], m[8], m[11] / scaleMultiplier));
                matrix.SetRow(3, new Vector4(0.0f, 0.0f, 0.0f, 1.0f));

                Matrix4x4 IJKToRAS = new Matrix4x4();
                IJKToRAS.SetRow(0, new Vector4(-1.0f, 0, 0, 0));
                IJKToRAS.SetRow(1, new Vector4(0, -1.0f, 0, 2));
                IJKToRAS.SetRow(2, new Vector4(0, 0, 1.0f, 0));
                IJKToRAS.SetRow(3, new Vector4(0.0f, 0.0f, 0.0f, 1.0f));

                Matrix4x4 matrixRAS = matrix * IJKToRAS;

                Vector3 translation = matrix.GetColumn(3);
                gameObject.transform.localPosition = new Vector3(-translation.x, translation.y, translation.z);
                Vector3 eulerAngles = matrix.rotation.eulerAngles;
                gameObject.transform.localRotation = Quaternion.Euler(eulerAngles.x, -eulerAngles.y, -eulerAngles.z);
            }
        }
        // Place this inside the loop if you only want to perform one loop per update cycle
        yield return null;
    }

    // -------------------- Send -------------------- 
    private void Send(Socket client, byte[] msg)
    {
        client.BeginSend(msg, 0, msg.Length, 0, new AsyncCallback(SendCallback), client);
    }

    private void SendCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.
            Socket client = (Socket)ar.AsyncState;

            // Complete sending the data to the remote device.
            int bytesSent = client.EndSend(ar);
            Debug.Log(String.Format("Sent {0} bytes to server.", bytesSent));

            // Signal that all bytes have been sent.
            sendDone.Set();
        }
        catch (Exception e)
        {
            Debug.Log(String.Format(e.ToString()));
        }
    }

    // ------------ Send Functions ------------ 
    // --- Send Transform --- 
    void SendTransformMessage(Transform objectTransform)
    {
        // Header
        // Header information:
        // Version 1
        // Type Transform
        // Device Name 
        // Time 0
        // Body size 30 bytes
        // 0001 Type:5452414E53464F524D000000 Name:4F63756C757352696674506F736974696F6E0000 00000000000000000000000000000030

        string hexHeader = "0001" + StringToHexString("TRANSFORM", 12) + StringToHexString(objectTransform.name, 20) + "00000000000000000000000000000030";

        // Body
        string m00Hex;
        string m01Hex;
        string m02Hex;
        string m03Hex;
        string m10Hex;
        string m11Hex;
        string m12Hex;
        string m13Hex;
        string m20Hex;
        string m21Hex;
        string m22Hex;
        string m23Hex;

        Matrix4x4 matrix = Matrix4x4.TRS(objectTransform.localPosition, objectTransform.localRotation, objectTransform.localScale);

        float m00 = matrix.GetRow(0)[0];
        byte[] m00Bytes = BitConverter.GetBytes(m00);
        float m01 = matrix.GetRow(0)[1];
        byte[] m01Bytes = BitConverter.GetBytes(m01);
        float m02 = matrix.GetRow(0)[2];
        byte[] m02Bytes = BitConverter.GetBytes(m02);
        float m03 = matrix.GetRow(0)[3];
        byte[] m03Bytes = BitConverter.GetBytes(m03 * scaleMultiplier);

        float m10 = matrix.GetRow(1)[0];
        byte[] m10Bytes = BitConverter.GetBytes(m10);
        float m11 = matrix.GetRow(1)[1];
        byte[] m11Bytes = BitConverter.GetBytes(m11);
        float m12 = matrix.GetRow(1)[2];
        byte[] m12Bytes = BitConverter.GetBytes(m12);
        float m13 = matrix.GetRow(1)[3];
        byte[] m13Bytes = BitConverter.GetBytes(m13 * scaleMultiplier);

        float m20 = matrix.GetRow(2)[0];
        byte[] m20Bytes = BitConverter.GetBytes(m20);
        float m21 = matrix.GetRow(2)[1];
        byte[] m21Bytes = BitConverter.GetBytes(m21);
        float m22 = matrix.GetRow(2)[2];
        byte[] m22Bytes = BitConverter.GetBytes(m22);
        float m23 = matrix.GetRow(2)[3];
        byte[] m23Bytes = BitConverter.GetBytes(m23 * scaleMultiplier);

        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(m00Bytes);
            Array.Reverse(m01Bytes);
            Array.Reverse(m02Bytes);
            Array.Reverse(m03Bytes);
            Array.Reverse(m10Bytes);
            Array.Reverse(m11Bytes);
            Array.Reverse(m12Bytes);
            Array.Reverse(m13Bytes);
            Array.Reverse(m20Bytes);
            Array.Reverse(m21Bytes);
            Array.Reverse(m22Bytes);
            Array.Reverse(m23Bytes);
        }
        m00Hex = BitConverter.ToString(m00Bytes).Replace("-", "");
        m01Hex = BitConverter.ToString(m01Bytes).Replace("-", "");
        m02Hex = BitConverter.ToString(m02Bytes).Replace("-", "");
        m03Hex = BitConverter.ToString(m03Bytes).Replace("-", "");
        m10Hex = BitConverter.ToString(m10Bytes).Replace("-", "");
        m11Hex = BitConverter.ToString(m11Bytes).Replace("-", "");
        m12Hex = BitConverter.ToString(m12Bytes).Replace("-", "");
        m13Hex = BitConverter.ToString(m13Bytes).Replace("-", "");
        m20Hex = BitConverter.ToString(m20Bytes).Replace("-", "");
        m21Hex = BitConverter.ToString(m21Bytes).Replace("-", "");
        m22Hex = BitConverter.ToString(m22Bytes).Replace("-", "");
        m23Hex = BitConverter.ToString(m23Bytes).Replace("-", "");

        string body = m00Hex + m10Hex + m20Hex + m01Hex + m11Hex + m21Hex + m02Hex + m12Hex + m22Hex + m03Hex + m13Hex + m23Hex;

        ulong crcULong = crcGenerator.Compute(StringToByteArray(body), 0, 0);
        CRC = crcULong.ToString("X16");

        string hexmsg = hexHeader + CRC + body;

        // Encode the data string into a byte array.
        byte[] msg = StringToByteArray(hexmsg);

        // Send the data through the socket.
        Send(socket, msg);
    }

    // --- Send Point --- 
    void SendPointMessage(Transform objectTransform)
    {
        // Header
        // Header information:
        // Version 1
        // Type Point
        // Device Name 
        // Time 0

        // Size 88 for one point, no support for full point list yet
        string hexHeader = "0001" + StringToHexString("POINT", 12) + StringToHexString(objectTransform.name, 20) + "00000000000000000000000000000088";

        // Body
        string xHex;
        string yHex;
        string zHex;

        float x = -objectTransform.localPosition.x * scaleMultiplier;
        float y = objectTransform.localPosition.y * scaleMultiplier;
        float z = objectTransform.localPosition.z * scaleMultiplier;

        byte[] xBytes = BitConverter.GetBytes(x);
        byte[] yBytes = BitConverter.GetBytes(y);
        byte[] zBytes = BitConverter.GetBytes(z);

        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(xBytes);
            Array.Reverse(yBytes);
            Array.Reverse(zBytes);
        }
        xHex = BitConverter.ToString(xBytes).Replace("-", "");
        yHex = BitConverter.ToString(yBytes).Replace("-", "");
        zHex = BitConverter.ToString(zBytes).Replace("-", "");

        // Default Slicer Fiducial Color
        string pointHeader = StringToHexString("TargetList-1", 64) + StringToHexString("GROUP_0", 32) + "FF0000FF";
        string diameter = "42340000";
        string imageID = StringToHexString("IMAGE_0", 20);

        string body = pointHeader + xHex + yHex + zHex + diameter + imageID;

        ulong crcULong = crcGenerator.Compute(StringToByteArray(body), 0, 0);
        CRC = crcULong.ToString("X16");

        string hexmsg = hexHeader + CRC + body;

        // Encode the data string into a byte array.
        byte[] msg = StringToByteArray(hexmsg);

        // Send the data through the socket.
        Send(socket, msg);
    }

    // --- Send Helpers ---
    static string StringToHexString(string inputString, int sizeInBytes)
    {
        if (inputString.Length > sizeInBytes)
        {
            inputString = inputString.Substring(0, sizeInBytes);
        }

        byte[] ba = Encoding.Default.GetBytes(inputString);
        string hexString = BitConverter.ToString(ba);
        hexString = hexString.Replace("-", "");
        hexString = hexString.PadRight(sizeInBytes * 2, '0');
        return hexString;
    }

    static byte[] StringToByteArray(string hex)
    {
        byte[] arr = new byte[hex.Length >> 1];

        for (int i = 0; i < (hex.Length >> 1); ++i)
        {
            arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
        }

        return arr;
    }

    static string ByteArrayToString(byte[] ba)
    {
        StringBuilder hex = new StringBuilder(ba.Length * 2);
        foreach (byte b in ba)
            hex.AppendFormat("{0:x2}", b);
        return hex.ToString();
    }

    static int GetHexVal(char hex)
    {
        int val = (int)hex;
        //For uppercase:
        return val - (val < 58 ? 48 : 55);
        //For lowercase:
        //return val - (val < 58 ? 48 : 87);
    }
}

public class CRC64
{
    private ulong[] _table;

    private ulong CmTab(int index, ulong poly)
    {
        ulong retval = (ulong)index;
        ulong topbit = (ulong)1L << (64 - 1);
        ulong mask = 0xffffffffffffffffUL;

        retval <<= (64 - 8);
        for (int i = 0; i < 8; i++)
        {
            if ((retval & topbit) != 0)
                retval = (retval << 1) ^ poly;
            else
                retval <<= 1;
        }
        return retval & mask;
    }

    private ulong[] GenStdCrcTable(ulong poly)
    {
        ulong[] table = new ulong[256];
        for (var i = 0; i < 256; i++)
            table[i] = CmTab(i, poly);
        return table;
    }

    private ulong TableValue(ulong[] table, byte b, ulong crc)
    {
        return table[((crc >> 56) ^ b) & 0xffUL] ^ (crc << 8);
    }

    public void Init(ulong poly)
    {
        _table = GenStdCrcTable(poly);
    }

    public ulong Compute(byte[] bytes, ulong initial, ulong final)
    {
        ulong current = initial;
        for (var i = 0; i < bytes.Length; i++)
        {
            current = TableValue(_table, bytes[i], current);
        }
        return current ^ final;

    }

}

// Receive Object
public class StateObject
{
    // Client socket.
    public Socket workSocket = null;
    // Size of receive buffer.
    public const int BufferSize = 4194304;
    //public const int BufferSize = 1024;
    
    // Receive buffer.
    public byte[] buffer = new byte[BufferSize];
    // Received data string.
    //public StringBuilder sb = new StringBuilder();
    public List<Byte> byteList = new List<Byte>();
    // OpenIGTLink Data Type
    public enum DataTypes { IMAGE = 0, TRANSFORM, STATUS };
    public DataTypes dataType;
    // Header read or not
    public bool headerRead = false;
    // Data Size read from header
    public int dataSize = -1;
    // Bytes of data read so far
    public int totalBytesRead = 0;
    // Transform Name
    public string name;
}