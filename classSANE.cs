using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace INSane
{
    class classSANE
    {
        internal partial struct SANE_IMAGE
        {
            public List<SANE_IMAGE_FRAME> Frames;
        }

        internal partial struct SANE_IMAGE_FRAME
        {
            public SANE_Parameter Params;
            public byte[] Data;
        }

        internal partial struct ImageDataWorkerState
        {
            public Bitmap bmp;
            public SANE_STATUS Status;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 2, CharSet = CharSet.Ansi)]
        internal partial struct SANE_Parameter
        {
            public SANE_FRAME format;
            public bool last_frame;
            public int bytes_per_line;
            public int pixels_per_line;
            public int lines;
            public int depth;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 2, CharSet = CharSet.Ansi)]
        internal partial struct NetworkDevice
        {
            public string name;
            public string vendor;
            public string model;
            public string type;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 2, CharSet = CharSet.Ansi)]
        internal partial struct NetworkDeviceOption
        {
            public string name;
            public string title;
            public string description;
            public int size;
            public int number;
            public int type;
            public int unit;
            public int SaneCapabilities;
            public uint constraint;
            public List<string> constraint_values;
        }

        internal enum NetworkCommand
        {
            Initialize = 0,
            GetDevices = 1,
            Open = 2,
            Close = 3,
            GetOptionDescriptors = 4,
            ControlOption = 5,
            GetParameters = 6,
            Start = 7,
            Cancel = 8,
            Exit = 10
        }

        internal enum SaneOptionAction : uint
        {
            Get,
            Set,
            Automatic
        }

        public enum SaneType
        {
            Boolean,
            Integer,
            Fixed,
            String,
            Button,
            Group
        }

        public enum SANE_FRAME
        {
            SANE_FRAME_GRAY = 0,
            SANE_FRAME_RGB = 1,
            SANE_FRAME_RED = 2,
            SANE_FRAME_GREEN = 3,
            SANE_FRAME_BLUE = 4 
        }

        public enum SANE_STATUS
        {
            Success = 0,
            Unsupported = 1,
            Canceled = 2,
            DeviceBusy = 3,
            Invalid = 4,
            EndOfFile = 5,
            Jammed = 6,
            NoDocuments = 7,
            CoverOpen = 8,
            IOError = 9,
            OutOfMemory = 10,
            AccessDenied = 11
        }

        internal enum SANE_BYTE_ORDER
        {
            SANE_NET_LITTLE_ENDIAN = 0x1234,
            SANE_NET_BIG_ENDIAN = 0x4321
        }

        internal enum SANE_CONSTRAINT_TYPE
        {
            SANE_CONSTRAINT_NONE = 0,
            SANE_CONSTRAINT_RANGE = 1,
            SANE_CONSTRAINT_WORD_LIST = 2,
            SANE_CONSTRAINT_STRING_LIST = 3
        }

        public string hostname;
        private readonly TcpClient socket;
        private readonly NetworkStream stream;
        public NetworkDevice networkDevice;
        private int networkDeviceHandle;
        private System.ComponentModel.BackgroundWorker ImageDataWorker;
        private ImageDataWorkerState ImageWorkerState;

        private int ImageData_Port = 0;
        private SANE_BYTE_ORDER ImageData_ByteOrder;

        public List<NetworkDeviceOption> networkDeviceOptions;
        public bool DEVICE_FEEDERENABLED = false;
        public bool DEVICE_DUPLEX = false;

        public classSANE(string _hostname, int _port, string _scanner)
        {
            SANE_STATUS status = SANE_STATUS.Invalid;
            int retry = 0;
            int max_retry = 0;

            do
            {
                int connection_step = 0;

                try
                {
                    // Connect to SANE Host
                    hostname = _hostname;

                    socket = new TcpClient(_hostname, _port);
                    stream = socket.GetStream();

                    connection_step++;

                    // Initialize SANE
                    SendWord((int)NetworkCommand.Initialize);
                    SendWord(CreateVersionCode(1, 0, 3));
                    SendString("insane");

                    status = (SANE_STATUS)ReadWord();

                    if (status != SANE_STATUS.Success)
                        throw new Exception();

                    int version = ReadWord();
                    Console.WriteLine(version);

                    connection_step++;

                    // Get Devices and pick one by deviceSelector ( autolocate )
                    SendWord((int)NetworkCommand.GetDevices);

                    status = (SANE_STATUS)ReadWord();

                    if (status != SANE_STATUS.Success)
                        throw new Exception();

                    int size = ReadWord();

                    for (int i = 0; i < size; i++)
                    {
                        if (ReadWord() == 0)
                        {
                            NetworkDevice networkDevice = new NetworkDevice();
                            networkDevice.name = ReadString();
                            networkDevice.vendor = ReadString();
                            networkDevice.model = ReadString();
                            networkDevice.type = ReadString();

                            if (networkDevice.name.Contains(_scanner))
                                this.networkDevice = networkDevice;
                        }
                    }

                    // Open the found Device
                    SendWord((int)NetworkCommand.Open);
                    SendString(networkDevice.name);

                    status = (SANE_STATUS)ReadWord();
                    networkDeviceHandle = ReadWord();
                    string resource = ReadString();

                    if (status != SANE_STATUS.Success)
                    {
                        networkDevice = new NetworkDevice();
                        throw new Exception();
                    }

                    connection_step++;

                    // Get Device Options
                    networkDeviceOptions = new List<NetworkDeviceOption>();

                    SendWord((int)NetworkCommand.GetOptionDescriptors);
                    SendWord(networkDeviceHandle);
                    int optionDescritorsLength = ReadWord();
                    for (int i = 0; i < optionDescritorsLength; i++)
                        if (ReadWord() == 0)
                        {
                            NetworkDeviceOption networkDeviceOption = new NetworkDeviceOption();
                            networkDeviceOption.name = ReadString();
                            networkDeviceOption.title = ReadString();
                            networkDeviceOption.description = ReadString();

                            networkDeviceOption.type = ReadWord();
                            networkDeviceOption.unit = ReadWord();
                            networkDeviceOption.size = ReadWord();
                            networkDeviceOption.SaneCapabilities = ReadWord();
                            networkDeviceOption.constraint = (uint)ReadWord();
                            networkDeviceOption.constraint_values = new List<string>();

                            int constraintLength = 0;
                            switch (networkDeviceOption.constraint)
                            {
                                case (int)SANE_CONSTRAINT_TYPE.SANE_CONSTRAINT_NONE:
                                    Console.WriteLine("No Constraints");
                                    break;
                                case (int)SANE_CONSTRAINT_TYPE.SANE_CONSTRAINT_WORD_LIST:
                                    constraintLength = ReadWord();
                                    for (int y = 0; y < constraintLength; y++)
                                        networkDeviceOption.constraint_values.Add(ReadWord().ToString());
                                    break;
                                case (int)SANE_CONSTRAINT_TYPE.SANE_CONSTRAINT_STRING_LIST:
                                    constraintLength = ReadWord();
                                    for (int y = 0; y < constraintLength; y++)
                                    {
                                        string constriant_string = ReadString();

                                        if(constriant_string.Trim().Length > 0)
                                            networkDeviceOption.constraint_values.Add(constriant_string);

                                        if (!DEVICE_FEEDERENABLED) DEVICE_FEEDERENABLED = constriant_string.ToLower().Contains("adf");
                                        if (!DEVICE_DUPLEX) DEVICE_DUPLEX = constriant_string.ToLower().Contains("duplex");
                                    }
                                    break;
                                case (int)SANE_CONSTRAINT_TYPE.SANE_CONSTRAINT_RANGE:
                                    for (int y = 0; y < 4; y++)
                                        networkDeviceOption.constraint_values.Add(ReadWord().ToString());
                                    break;
                            }

                            networkDeviceOptions.Add(networkDeviceOption);
                        }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                    status = SANE_STATUS.Invalid;
                    retry++;

                    switch (connection_step)
                    {
                        case 0:
                            MessageBox.Show(null, string.Format("Es konnte keine Verbindung zum Host {0} aufgebaut werden", hostname), "Verbindungsfehler", MessageBoxButtons.OK);
                        break;
                        case 1:
                            MessageBox.Show(null, string.Format("Fehler beim initialisieren des Scannerservers"), "Verbindungsfehler", MessageBoxButtons.OK);
                        break; 
                        case 2:
                            MessageBox.Show(null, string.Format("Der Scanner {0} konnte nicht gefunden werden. Bitte achten Sie darauf, dass der Scanner eingeschaltet und verbunden ist. Sollte dies der Fall sein, starten Sie den Scanner bitte neu und wiederholen Sie dann den Scanvorgang", _scanner), "Verbindungsfehler", MessageBoxButtons.OK);
                        break;
                        case 3:
                            MessageBox.Show(null, string.Format("Fehler beim Abrufen der Scannereigenschaften für {0]", _scanner), "Verbindungsfehler", MessageBoxButtons.OK);
                            break;
                    }
                }
            } while (status != SANE_STATUS.Success && retry < max_retry);            
        }

        private static int CreateVersionCode(int major, int minor, int build)
        {
            int value = (major & 0xff) << 24;
            value += (minor & 0xff) << 16;
            value += (build & 0xffff) << 0;
            return value;
        }

        internal void SendWord(int word)
        {
            if (stream != null)
            {
                stream.WriteByte((byte)((word >> 24) & 0xff));
                stream.WriteByte((byte)((word >> 16) & 0xff));
                stream.WriteByte((byte)((word >> 8) & 0xff));
                stream.WriteByte((byte)((word >> 0) & 0xff));
            }
        }

        internal void SendString(string value, int size = 255)
        {
            SendWord(size);
            for (int i = 0; i < size; ++i)
            {
                int val = i < value.Length ? value[i] : 0;
                stream.WriteByte((byte)val);
            }
        }

        internal int ReadWord()
        {
            if (stream != null)
            {
                int value = 0;
                value += (stream.ReadByte() << 24);
                value += (stream.ReadByte() << 16);
                value += (stream.ReadByte() << 8);
                value += (stream.ReadByte() << 0);
                return value;
            }
            return 0;
        }

        internal string ReadString()
        {
            var str = new StringBuilder();

            int size = ReadWord();
            bool add = true;

            if (size > 0)
                for (int i = 0; i < size; ++i)
                {
                    int byt = stream.ReadByte();
                    var chr = (char)byt;
                    if (byt == 0)
                        add = false;
                    if (add)
                        str.Append(chr);
                }

            return str.ToString();
        }

        internal SANE_STATUS Net_Control_Option(string optionName, string value)
        {
            NetworkDeviceOption option = networkDeviceOptions.Find(v => v.name == optionName);

            if (option.name != null)
            {
                int optionIndex = networkDeviceOptions.IndexOf(option);

                SendWord((int)NetworkCommand.ControlOption);
                SendWord(networkDeviceHandle);
                SendWord(optionIndex);
                SendWord((int)SaneOptionAction.Set);
                SendWord(option.type);
                SendWord(option.size);

                if (option.type == (int)SaneType.String)
                    SendString(value, option.size);
                else if(option.type == (int)SaneType.Boolean)
                {
                    SendWord(1);
                    SendWord(value == "True" ? 1 : 0);
                }
                else
                {
                    SendWord(1); // Requested pseudo Array - Size
                    SendWord(Convert.ToInt32(value));
                }

                SANE_STATUS status = (SANE_STATUS)ReadWord();

                var info = ReadWord();
                var valueType = (SaneType)ReadWord();
                int valueSize = ReadWord();

                if (option.type == (int)SaneType.String)
                    ReadString();
                else
                {
                    ReadWord();
                    ReadWord();
                }

                string resource = ReadString();

                return status;
            }
            else
                return SANE_STATUS.Unsupported;
        }

        internal SANE_Parameter Net_Get_Parameters()
        {
            SendWord((int)NetworkCommand.GetParameters);
            SendWord(networkDeviceHandle);

            SANE_STATUS status = (SANE_STATUS)ReadWord();

            SANE_Parameter SANEparams = new SANE_Parameter();
            SANEparams.format = (SANE_FRAME)ReadWord();
            SANEparams.last_frame = ReadWord() == 1;
            SANEparams.bytes_per_line = ReadWord();
            SANEparams.pixels_per_line = ReadWord();
            SANEparams.lines = ReadWord();
            SANEparams.depth = ReadWord();

            return SANEparams;
        }

        internal SANE_STATUS Net_Start()
        {
            SendWord((int)NetworkCommand.Start);
            SendWord(networkDeviceHandle);

            SANE_STATUS Status = (SANE_STATUS)ReadWord();
            ImageData_Port = ReadWord();
            ImageData_ByteOrder = (SANE_BYTE_ORDER)ReadWord();
            string resource = ReadString();

            return Status;
        }

        internal void Net_Close()
        {
            if (networkDevice.name != null)
            {
                SendWord((int)NetworkCommand.Close);
                SendWord(networkDeviceHandle);

                int dummy = ReadWord();
            }
        }

        internal void Net_Cancle()
        {
            if (networkDevice.name != null)
            {
                SendWord((int)NetworkCommand.Cancel);
                SendWord(networkDeviceHandle);

                int dummy = ReadWord();
            }
        }

        internal void Net_Exit()
        {
            SendWord((int)NetworkCommand.Exit);

            if (socket != null)
            {
                socket.Close();
                socket.Dispose();
            }
        }

        public SANE_STATUS AcquireImage(ref Bitmap bmp)
        {
            ImageWorkerState = new ImageDataWorkerState();
            ImageDataWorker = new System.ComponentModel.BackgroundWorker();
            ImageDataWorker.DoWork += ImageDataWorker_DoWork;
            ImageDataWorker.RunWorkerCompleted += ImageDataWorker_RunWorkerCompleted;

            if (!ImageDataWorker.IsBusy)
            {
                ImageWorkerState.bmp = bmp;
                ImageWorkerState.Status = SANE_STATUS.Success;
                ImageDataWorker.RunWorkerAsync(ImageWorkerState);
            }

            while (networkDevice.name != null && ImageDataWorker.IsBusy)
            {
                System.Windows.Forms.Application.DoEvents();
                System.Threading.Thread.Sleep(250);
            }

            if (networkDevice.name != null)
            {
                bmp = ImageWorkerState.bmp;
                return ImageWorkerState.Status;
            }
            else
                return SANE_STATUS.IOError;
        }

        internal SANE_IMAGE_FRAME AcquireFrame()
        {
            TcpClient Data_TCPClient = new TcpClient(hostname, ImageData_Port);
            Data_TCPClient.ReceiveBufferSize = 32768;
            Data_TCPClient.SendBufferSize = 32768;
            SANE_IMAGE_FRAME Frame;

            Frame.Params = Net_Get_Parameters();

            NetworkStream n_imagestream = null;
            MemoryStream m_imagestream = null;

            try
            {
                uint TransferredBytes = 0;
                n_imagestream = Data_TCPClient.GetStream();

                m_imagestream = new MemoryStream();
                byte[] ImageBuf = new byte[Data_TCPClient.ReceiveBufferSize - 1];

                uint Expected_Total_Bytes = 0;
                if (Frame.Params.lines > 0)
                    Expected_Total_Bytes = (uint)(Frame.Params.lines * Frame.Params.bytes_per_line);

                do
                {
                    byte[] buf = new byte[4];
                    int bytes_read = 0;

                    do
                    {
                        int n = n_imagestream.Read(buf, bytes_read, 1);
                        bytes_read += n;
                    } while (bytes_read < 4);

                    uint data_length = BitConverter.ToUInt32(buf, 0);
                    data_length = SwapEndian(data_length);

                    if (data_length == uint.MinValue + 0x7FFFFFFF)
                    {
                        if (TransferredBytes == 0)
                            throw new Exception();
                        break;
                    }
                    else if (Expected_Total_Bytes > 0 && TransferredBytes >= Expected_Total_Bytes)
                    {
                        break;
                    }

                    if (data_length < Data_TCPClient.ReceiveBufferSize)
                    {
                        uint TotalBytes = 0U;
                        do
                        {
                            uint ImageBytes = (uint)n_imagestream.Read(ImageBuf, 0, (int)(data_length - TotalBytes));
                            m_imagestream.Write(ImageBuf, 0, (int)ImageBytes);
                            TotalBytes += ImageBytes;
                        }
                        while (TotalBytes < data_length);
                        TransferredBytes += TotalBytes;
                    }
                    else
                    {
                        break;
                    }
                } while (true);

                if (TransferredBytes < Expected_Total_Bytes)
                {
                    var Filler = new byte[Expected_Total_Bytes - TransferredBytes + 1];
                    m_imagestream.Write(Filler, 0, Filler.Length);
                    Filler = new byte[0];
                }

                ulong TransferredMbits = TransferredBytes * 8UL / 1024 / 1024;
                Frame.Data = m_imagestream.ToArray();

                return Frame;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                try { 
                    if (m_imagestream != null) m_imagestream.Close();
                    if (n_imagestream != null) n_imagestream.Close();
                    if (Data_TCPClient != null) Data_TCPClient.Client.Shutdown(System.Net.Sockets.SocketShutdown.Both);
                    if (Data_TCPClient != null) Data_TCPClient.Close();
                } 
                catch (Exception ex) 
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private ColorPalette Get1bitGrayScalePalette()
        {
            var bmp = new Bitmap(1, 1, PixelFormat.Format1bppIndexed);
            ColorPalette palette = bmp.Palette;
            palette.Entries[0] = Color.White;
            palette.Entries[1] = Color.Black;
            return palette;
        }

        private ColorPalette Get8bitGrayScalePalette()
        {
            var bmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed);
            ColorPalette palette = bmp.Palette;
            for (int i = 0, loopTo = palette.Entries.Length - 1; i <= loopTo; i++)
                palette.Entries[i] = Color.FromArgb(i, i, i);
            return palette;
        }
        
        private void SwapImageBytes(ref byte[] bytes)
        {
            for (uint i = 0U, loopTo = (uint)(bytes.Length - 1); i <= loopTo; i += 2U)
            {
                if (i < bytes.Length - 1)
                {
                    byte b = bytes[(int)i];
                    bytes[(int)i] = bytes[(int)(i + 1L)];
                    bytes[(int)(i + 1L)] = b;
                }
            }
        }

        public void RGBtoBGR(ref byte[] bytes, int BitsPerColor)
        {
            switch (BitsPerColor)
            {
                case 1:
                    throw new Exception("1bpp images are not yet supported");
                case 8:
                    for (uint i = 0U, loopTo = (uint)(bytes.Length - 1); i <= loopTo; i += 3U)
                    {
                        if (i < bytes.Length - 2)
                        {
                            byte R = bytes[(int)i];
                            bytes[(int)i] = bytes[(int)(i + 2L)];
                            bytes[(int)(i + 2L)] = R;
                        }
                    }

                    break;
                case 16:
                    for (uint i = 0U, loopTo1 = (uint)(bytes.Length - 1); i <= loopTo1; i += 6U)
                    {
                        if (i < bytes.Length - 5)
                        {
                            byte R1 = bytes[(int)i];
                            byte R2 = bytes[(int)(i + 1L)];
                            bytes[(int)i] = bytes[(int)(i + 4L)];
                            bytes[(int)(i + 1L)] = bytes[(int)(i + 5L)];
                            bytes[(int)(i + 4L)] = R1;
                            bytes[(int)(i + 5L)] = R2;
                        }
                    }
                    break;
            }
        }


        private void ImageDataWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            SANE_STATUS Status = SANE_STATUS.Success;
            SANE_IMAGE SANEImage = new SANE_IMAGE();
            SANEImage.Frames = new List<SANE_IMAGE_FRAME>();
            int CurrentFrame = 0;

            try
            {
                do
                {
                    Status = Net_Start();
                    if (Status == SANE_STATUS.Success)
                    {
                        SANEImage.Frames.Add(AcquireFrame());

                        if (SANEImage.Frames[SANEImage.Frames.Count-1].Params.last_frame)
                            break;
                        else
                            CurrentFrame++;
                    }
                } while (Status == SANE_STATUS.Success);

                if(Status == SANE_STATUS.Success)
                {
                    ImageWorkerState.bmp = null;
                    BitmapData bmp_data;

                    try
                    {
                        SANE_IMAGE_FRAME SANEImageFrame = SANEImage.Frames[0];
                        int h = SANEImageFrame.Params.lines;
                        int w = SANEImageFrame.Params.pixels_per_line;
                        int stride = SANEImageFrame.Params.bytes_per_line;

                        if (h < 0)
                            h = SANEImage.Frames[0].Data.Length / stride;

                        Rectangle bounds = new Rectangle(0, 0, w, h);
                        PixelFormat PixelFormat = PixelFormat.Format1bppIndexed;
                        ColorPalette Palette = null;

                        switch (SANEImage.Frames[0].Params.format)
                        {
                            case SANE_FRAME.SANE_FRAME_GRAY:
                                switch (SANEImageFrame.Params.depth)
                                {
                                    case 1:
                                        PixelFormat = PixelFormat.Format1bppIndexed;
                                        Palette = Get1bitGrayScalePalette();
                                        break;
                                    case 8:
                                        PixelFormat = PixelFormat.Format8bppIndexed;
                                        Palette = Get8bitGrayScalePalette();
                                        break;
                                    case 16:
                                        if (ImageData_ByteOrder == SANE_BYTE_ORDER.SANE_NET_BIG_ENDIAN) SwapImageBytes(ref SANEImageFrame.Data);
                                        PixelFormat = PixelFormat.Format16bppGrayScale;
                                        break;
                                }
                                break;
                            case SANE_FRAME.SANE_FRAME_RGB:
                                RGBtoBGR(ref SANEImageFrame.Data, SANEImageFrame.Params.depth);
                                switch (SANEImageFrame.Params.depth)
                                {
                                    case 1:
                                        Status = SANE_STATUS.Invalid;
                                        break;
                                    case 8:
                                        PixelFormat = PixelFormat.Format24bppRgb;
                                        break;
                                    case 16:
                                        if (ImageData_ByteOrder == SANE_BYTE_ORDER.SANE_NET_BIG_ENDIAN) SwapImageBytes(ref SANEImageFrame.Data);
                                        PixelFormat = PixelFormat.Format48bppRgb;
                                        break;
                                }
                                break;
                        }

                        if (Status == SANE_STATUS.Success)
                        {
                            try
                            {
                                ImageWorkerState.bmp = new Bitmap(w, h, PixelFormat);
                            }
                            catch (Exception)
                            {
                                throw;
                            }

                            if (Palette != null) ImageWorkerState.bmp.Palette = Palette;
                            bmp_data = ImageWorkerState.bmp.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat);

                            if(stride<bmp_data.Stride)
                                for (int r = 0, loopTo = h - 1; r <= loopTo; r++)
                                    Marshal.Copy(SANEImageFrame.Data, stride * r, bmp_data.Scan0 + bmp_data.Stride * r, stride);
                            else if(stride > bmp_data.Stride)
                                for (int r = 0, loopTo = h - 1; r <= loopTo; r++)
                                    Marshal.Copy(SANEImageFrame.Data, stride * r, bmp_data.Scan0 + (bmp_data.Stride * r), bmp_data.Stride);
                            else
                                Marshal.Copy(SANEImageFrame.Data, 0, bmp_data.Scan0, SANEImageFrame.Data.Length);

                            ImageWorkerState.bmp.UnlockBits(bmp_data);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.Message);
                        Status = SANE_STATUS.IOError;
                    }
                    finally
                    {
                        bmp_data = null;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.Write(ex.Message);
                Status = SANE_STATUS.IOError;
            }
            finally
            {
                ImageWorkerState.Status = Status;
                e.Result = ImageWorkerState;
            }
        }

        private void ImageDataWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (object.ReferenceEquals(e, typeof(ImageDataWorkerState)))
                {
                    ImageDataWorkerState State = (ImageDataWorkerState)e.Result;
                    ImageWorkerState = State;
                }

                if (e.Error is object)
                    return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        internal uint SwapEndian(uint _UInt32)
        {
            uint n;
            n = (uint)((_UInt32 & 0xFFL) << 24);
            n = (uint)(n + ((_UInt32 & 0xFF00L) << 8));
            n = (uint)(n + ((_UInt32 & 0xFF0000L) >> 8));
            n = (uint)(n + ((_UInt32 & int.MinValue + 0x7F000000) >> 24));
            return n;
        }
    }
}
