// TWAIN Specification: https://twain.org/wp-content/uploads/2016/03/TWAIN-2.2-Spec.pdf

using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static INSane.classSANE;
using System.Configuration;
using System.Reflection;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace INSane
{
    internal partial struct TWAINImage
    {
        public TW_IMAGEINFO ImageInfo;
        public TW_IMAGELAYOUT ImageLayout;
        public uint TotalBytes;
        public uint BytesTransferred;
        public uint LinesTransferred;
    }

    internal partial struct TWAINJob
    {
        public TW_PENDINGXFERS PendingXfers;
        public TWAINImage CurrentImage;
        public int ImagesXferred;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2, CharSet = CharSet.Ansi)]
    public partial struct TW_USERINTERFACE
    {
        public ushort ShowUI;
        public ushort ModalUI;
        public IntPtr hParent;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2, CharSet = CharSet.Ansi)]
    public partial struct TW_ONEVALUE_FIX32
    {
        public ushort ItemType;
        public TW_FIX32 Item;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2, CharSet = CharSet.Ansi)]
    public partial struct TW_ONEVALUE
    {
        public ushort ItemType;
        public int Item;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2, CharSet = CharSet.Ansi)]
    public partial struct TW_FRAME
    {
        public TW_FIX32 Left;
        public TW_FIX32 Top;
        public TW_FIX32 Right;
        public TW_FIX32 Bottom;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2, CharSet = CharSet.Ansi)]
    public partial struct TW_IMAGELAYOUT
    {
        public TW_FRAME Frame;
        public uint DocumentNumber;
        public uint PageNumber;
        public uint FrameNumber;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2, CharSet = CharSet.Ansi)]
    public partial struct TW_SETUPMEMXFER
    {
        public uint MinBufSize;
        public uint MaxBufSize;
        public uint Preferred;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2, CharSet = CharSet.Ansi)]
    public partial struct TW_CAPABILITY
    {
        public CAP Cap;
        public ushort ConType;
        public IntPtr hContainer;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2, CharSet = CharSet.Ansi)]
    public partial struct TW_EVENT
    {
        public IntPtr pEvent;
        public MSG TWMessage;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2, CharSet = CharSet.Ansi)]
    public partial struct TW_FIX32
    {
        public short Whole;
        public ushort Frac;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2, CharSet = CharSet.Ansi)]
    public partial struct TW_IMAGEINFO
    {
        public TW_FIX32 XResolution;
        public TW_FIX32 YResolution;
        public int ImageWidth;
        public int ImageLength;
        public short SamplesPerPixel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public short[] BitsPerSample;
        public short BitsPerPixel;
        public ushort Planar;
        public short Compression;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2, CharSet = CharSet.Ansi)]
    public partial struct TW_PENDINGXFERS
    {
        public short Count;
        public uint EOJ;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2, CharSet = CharSet.Ansi)]
    public partial struct TW_VERSION
    {
        public ushort MajorNum;
        public ushort MinorNum;
        public ushort Language;
        public ushort Country;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 34)]
        public string Info;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2, CharSet = CharSet.Ansi)]
    public partial struct TW_IDENTITY
    {
        public uint Id;
        public TW_VERSION Version;
        public ushort ProtocolMajor;
        public ushort ProtocolMinor;
        public uint SupportedGroups;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 34)]
        public string Manufacturer;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 34)]
        public string ProductFamily;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 34)]
        public string ProductName;
    }

    public enum TWSC
    {
        None = 0,
        DSM_Pre_Session = 1,
        DSM_Loaded = 2,
        DSM_Opened = 3,
        DS_Opened = 4,
        DS_Enabled = 5,
        DS_Xfer_Ready = 6,
        DS_Xfer_Active = 7
    }

    public enum TWRC : short
    {
        TWRC_SUCCESS = 0,
        TWRC_FAILURE = 1,
        TWRC_CANCEL = 3,
        TWRC_DSEVENT = 4,
        TWRC_NOTDSEVENT  = 5,
        TWRC_XFERDONE = 6
    }

    public enum TWTY : ushort
    {
        TWTY_UINT16,
        TWTY_BOOL,
        TWTY_FIX32
    }

    public enum DG : uint
    {
        DG_CONTROL = 1,
        DG_IMAGE = 2
    }

    public enum DAT : uint
    {
        DAT_NULL = 0U,
        DAT_CAPABILITY = 0x1U,            // /* TW_CAPABILITY                        */
        DAT_EVENT = 0x2U,                 // /* TW_EVENT                             */
        DAT_IDENTITY = 0x3U,              // /* TW_IDENTITY                          */
        DAT_PENDINGXFERS = 0x5U,          // /* TW_PENDINGXFERS                      */
        DAT_SETUPMEMXFER = 0x6U,          // /* TW_SETUPMEMXFER                      */
        DAT_STATUS = 0x8U,                // /* TW_STATUS                            */
        DAT_USERINTERFACE = 0x9U,         // /* TW_USERINTERFACE                     */
        DAT_IMAGEINFO = 0x101U,           // /* TW_IMAGEINFO                         */
        DAT_IMAGELAYOUT = 0x102U,         // /* TW_IMAGELAYOUT                       */
        DAT_IMAGEMEMXFER = 0x103U,        // /* TW_IMAGEMEMXFER                      */
        DAT_IMAGENATIVEXFER = 0x104U,     // /* TW_UINT32 loword is hDIB, PICHandle  */
    }

    public enum MSG : ushort
    {
        MSG_NULL = 0x0,
        MSG_GET = 0x1,
        MSG_GETCURRENT = 0x2,
        MSG_GETDEFAULT = 0x3,
        MSG_SET = 0x6,
        MSG_RESET = 0x7,
        MSG_XFERREADY = 0x101,
        MSG_CLOSEDSREQ = 0x102,
        MSG_CLOSEDSOK = 0x103,
        MSG_OPENDSM = 0x301,
        MSG_CLOSEDSM = 0x302,
        MSG_OPENDS = 0x401,
        MSG_CLOSEDS = 0x402,
        MSG_DISABLEDS = 0x501,
        MSG_ENABLEDS = 0x502,
        MSG_ENABLEDSUIONLY = 0x503,
        MSG_PROCESSEVENT = 0x601,
        MSG_ENDXFER = 0x701
    }

    public enum CAP : ushort
    {
        ICAP_XRESOLUTION = 0x1118,
        ICAP_YRESOLUTION = 0x1119,
        ICAP_UNITS = 0x102,
        ICAP_XFERMECH = 0x103,
        CAP_UICONTROLLABLE = 0x100E,
        CAP_DUPLEXENABLED = 0x1013
    }

    public enum GlobalAllocFlags : int
    {
        GMEM_FIXED = 0x0,
        GMEM_MOVEABLE = 0x2,
        GMEM_ZEROINIT = 0x40,
        GPTR = 0x40,
        GHND = GMEM_MOVEABLE & GMEM_ZEROINIT
    }

    public class classTWAIN
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GlobalAlloc(int flags, int size);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr IsDialogMessage(IntPtr hDlg, IntPtr lpMsg);

        public ushort TWON_ONEVALUE = 5;
        public short TWUN_INCHES = 0;

        public delegate void Message_From_DSEventHandler(IntPtr _pOrigin, IntPtr _pDest, uint _DG, uint _DAT, ushort _MSG, IntPtr _pData);
        public event Message_From_DSEventHandler Message_From_DS;

        private TWAINJob CurrentJob;
        private TW_IDENTITY TWAINIdentity;
        private TW_IDENTITY AppIdentity;
        private TWSC TWAINState;
        private classSANE SANE;
        private formScan FormScan;
        private formProgress FormProgress;
        private string SANE_Host;

        private double CONTROL_RESOLUTION = 150.0;
        private int CONTROL_DUPLEXENABLED = 0;

        public string CTX_REGISTRY_BASE = @"SOFTWARE\Citrix\Ica\Session";

        public classTWAIN()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);

            if (config.AppSettings.Settings["configRedirect"] != null && config.AppSettings.Settings["configRedirect"].Value.Length > 0) { 
                if (File.Exists(config.AppSettings.Settings["configRedirect"].Value)) {
                    config = ConfigurationManager.OpenExeConfiguration(config.AppSettings.Settings["configRedirect"].Value);
                }
            }

            string ClientName = (getClientNameFromRegistry() != null ? getClientNameFromRegistry() : Environment.MachineName);
            SANE_Host = (config.AppSettings.Settings[ClientName] != null ? config.AppSettings.Settings[ClientName].Value : ClientName);

            TWAINState = TWSC.DSM_Opened;

            TWAINIdentity = new TW_IDENTITY();
            TWAINIdentity.Id = 10;
            TWAINIdentity.Version.MajorNum = 1;
            TWAINIdentity.Version.MinorNum = 9;
            TWAINIdentity.Version.Language = 6; // GERMAN
            TWAINIdentity.Version.Country = 49; // GERMANY
            TWAINIdentity.Version.Info = TWAINIdentity.Version.MajorNum + "." + TWAINIdentity.Version.MinorNum;
            TWAINIdentity.ProtocolMajor = 1;
            TWAINIdentity.ProtocolMinor = 9;
            TWAINIdentity.SupportedGroups = (uint)(DG.DG_IMAGE | DG.DG_CONTROL);
            TWAINIdentity.Manufacturer = "Fry";
            TWAINIdentity.ProductFamily = "FryEnginge3";
            TWAINIdentity.ProductName = "InSane";

            FormScan = new formScan();
            FormProgress = new formProgress();
        }

        public string getClientNameFromRegistry()
        {
            try
            {
                RegistryKey localKey32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                if (localKey32 != null && localKey32.OpenSubKey(CTX_REGISTRY_BASE) != null)
                {
                    string[] sessions = localKey32.OpenSubKey(CTX_REGISTRY_BASE).GetSubKeyNames();

                    foreach (string session in sessions)
                    {
                        if (Regex.IsMatch(session, @"^\d+$"))
                        {
                            string username = localKey32.OpenSubKey(CTX_REGISTRY_BASE + "\\" + session + "\\Connection").GetValue("UserName", "UserName").ToString();
                            if (username == Environment.UserName)
                            {
                                return (string)localKey32.OpenSubKey(CTX_REGISTRY_BASE + "\\" + session + "\\Connection").GetValue("ClientName", "ClientName");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            { 
                Console.WriteLine(ex.Message);
                return Environment.MachineName;
            }  

            return null;
        }

        public TW_FIX32 FloatToFIX32(double _float)
        {
            TW_FIX32 _FIX32;
            bool sign = (_float < 0);
            int value = Convert.ToInt32((_float * 65536.0d) + (sign ? -0.5d : 0.5d));

            _FIX32.Whole = Convert.ToInt16(value >> 16);
            _FIX32.Frac = (ushort)(value >> 0xFFF);

            return _FIX32;
        }

        private static uint GetChecksum(string s)
        {
            uint checksum = 0;
            foreach (char c in s)
            {
                checksum += Convert.ToByte(c);
            }
            return checksum;
        }

        private void Send_TWAIN_Message(TW_IDENTITY _Origin, TW_IDENTITY _Dest, DG _DG, DAT _DAT, MSG _MSG, object _Data)
        {
            IntPtr _pOrigin = GlobalAlloc((int)GlobalAllocFlags.GHND, Marshal.SizeOf(_Origin));
            IntPtr _pDest = GlobalAlloc((int)GlobalAllocFlags.GHND, Marshal.SizeOf(_Dest));

            Marshal.StructureToPtr(_Origin, _pOrigin, true);
            Marshal.StructureToPtr(_Dest, _pDest, true);

            Message_From_DS?.Invoke(_pOrigin, _pDest, (uint)_DG, (uint)_DAT, (ushort)_MSG, IntPtr.Zero);
        }

        public short Message_To_DS(IntPtr _pOrigin, DG _DG, DAT _DAT, MSG _MSG, IntPtr _pData, string _dsOrigin)
        {
            switch (_DG)
            {
                case DG.DG_CONTROL:
                    switch(_DAT)
                    {
                        case DAT.DAT_CAPABILITY:
                            return (short)DG_CONTROL__DAT_CAPABILITY(_MSG, _pData);
                        case DAT.DAT_IDENTITY:
                            return (short)DG_CONTROL__DAT_IDENTITY(_pOrigin, _MSG, _pData, _dsOrigin);
                        case DAT.DAT_USERINTERFACE:
                            return (short)DG_CONTROL__DAT_USERINTERFACE(_MSG, _pData, _dsOrigin);
                        case DAT.DAT_PENDINGXFERS:
                            return (short)DG_CONTROL__DAT_PENDINGXFERS(_MSG, _pData);
                        case DAT.DAT_SETUPMEMXFER:
                            return (short)DG_CONTROL__DAT_SETUPMEMXFER(_MSG, _pData);
                        case DAT.DAT_EVENT:
                            TW_EVENT e = (TW_EVENT)Marshal.PtrToStructure(_pData, typeof(TW_EVENT));
                            e.TWMessage = MSG.MSG_NULL;

                            if (e.pEvent == IntPtr.Zero)
                                return (short)TWRC.TWRC_FAILURE;
                            else
                            {
                                IntPtr hWnd = Marshal.ReadIntPtr(e.pEvent);
                                Marshal.StructureToPtr(e, _pData, true);

                                if (hWnd == FormScan.Handle)
                                {
                                    IsDialogMessage(hWnd, e.pEvent);
                                    return (short)TWRC.TWRC_DSEVENT;
                                }
                                else
                                    return (short)TWRC.TWRC_NOTDSEVENT;
                            }
                        default:
                            break;
                    }
                break;
                case DG.DG_IMAGE:
                    switch (_DAT) {
                        case DAT.DAT_IMAGEINFO:
                            return (short)DG_IMAGE__DAT_IMAGEINFO(_MSG, _pData);
                        case DAT.DAT_IMAGELAYOUT:
                            return (short)DG_IMAGE__DAT_IMAGELAYOUT(_MSG, _pData);
                        case DAT.DAT_IMAGENATIVEXFER:
                            return (short)DG_IMAGE__DAT_IMAGENATIVEXFER(_MSG, _pData, _dsOrigin);
                        default:
                            break;
                    }
                break;
                default:
                    return (short)(TWRC.TWRC_FAILURE);
            }

            return (short)(TWRC.TWRC_FAILURE);
        }

        private TWRC DG_IMAGE__DAT_IMAGENATIVEXFER(MSG _MSG, IntPtr _pData, string _dsOrigin)
        {
            if (SANE == null)
            {
                SANE = new classSANE(SANE_Host, 6566, _dsOrigin);

                FormScan.SetSANEConnection(SANE);
                FormScan.SetHostInformation();
                FormScan.SetFormControls();
                FormScan.SetUserDefaults();
            }

            TWAINState = TWSC.DS_Xfer_Active;
            Bitmap bmp = null;

            if (!FormProgress.Visible)
                FormProgress.Show();

            FormScan.SetControlsEnables(false);

            SANE_STATUS Status = SANE.AcquireImage(ref bmp);
            FormProgress.SetPages(CurrentJob.ImagesXferred);

            if (Status == SANE_STATUS.Success)
            {
                CurrentJob.PendingXfers.Count = 1;
                CurrentJob.ImagesXferred += 1;

                if (bmp != null)
                {
                    MemoryStream bmpStream = new MemoryStream();
                    bmp.Save(bmpStream, System.Drawing.Imaging.ImageFormat.Bmp);
                    byte[] B = bmpStream.ToArray();

                    bmp.Dispose();
                    bmpStream.Dispose();

                    IntPtr hBmp = GlobalAlloc((int)GlobalAllocFlags.GHND, B.Length - 14);
                    Marshal.Copy(B, 14, hBmp, B.Length - 14);

                    Array.Resize(ref B, 0);
                    Marshal.StructureToPtr(hBmp, _pData, true);

                    return TWRC.TWRC_XFERDONE;
                }
                else
                    throw new Exception("Status GOOD but no Bitmap !");
            }
            else
            {
                if(Status == SANE_STATUS.NoDocuments && CurrentJob.ImagesXferred == 0)
                    MessageBox.Show("Der Einzug des Scanners ist leer !");
                else if(Status == SANE_STATUS.Jammed)
                    MessageBox.Show("Papierstau im Scanner !");
                else if(Status == SANE_STATUS.IOError)
                    MessageBox.Show("Fehler bei Dateiübertragung !");

                SANE.DEVICE_FEEDERENABLED = false;
                return TWRC.TWRC_CANCEL;
            }
        }

        private TWRC DG_IMAGE__DAT_IMAGELAYOUT(MSG _MSG, IntPtr _pData)
        {
            Marshal.StructureToPtr(CurrentJob.CurrentImage.ImageLayout, _pData, true);
            return TWRC.TWRC_SUCCESS;
        }

        private TWRC DG_CONTROL__DAT_PENDINGXFERS(MSG _MSG, IntPtr _pData)
        {
            switch (_MSG)
            {
                case MSG.MSG_ENDXFER:
                    CurrentJob.CurrentImage = new TWAINImage();
                    TWAINState = TWSC.DS_Xfer_Ready;

                    if(SANE.DEVICE_FEEDERENABLED)
                        CurrentJob.PendingXfers.Count = -1;
                    else
                        CurrentJob.PendingXfers.Count = 0;
                    
                    if(CurrentJob.PendingXfers.Count == 0)
                    {
                        TWAINState = TWSC.DS_Enabled;
                        CurrentJob.ImagesXferred = 0;

                        if(!FormScan.Visible)
                            SANE_Close();

                        FormProgress.Hide();
                        FormScan.SetControlsEnables(true);
                    }

                    Marshal.StructureToPtr(CurrentJob.PendingXfers, _pData, true);
                    return TWRC.TWRC_SUCCESS;
                default:
                    return TWRC.TWRC_FAILURE;
            }
        }

        private TWRC DG_IMAGE__DAT_IMAGEINFO(MSG _MSG, IntPtr _pData)
        {
            Marshal.StructureToPtr(CurrentJob.CurrentImage.ImageInfo, _pData, true);
            return TWRC.TWRC_SUCCESS;
        }

        private TWRC DG_CONTROL__DAT_USERINTERFACE(MSG _MSG, IntPtr _pData, string _dsOrigin)
        {
            switch(_MSG)
            {
                case MSG.MSG_ENABLEDS:
                    if(TWAINState != TWSC.DS_Opened)
                        return TWRC.TWRC_FAILURE;
                    else
                    {
                        if (FormScan == null) FormScan = new formScan();

                        TW_USERINTERFACE UserInterface = (TW_USERINTERFACE) Marshal.PtrToStructure(_pData, typeof(TW_USERINTERFACE));
                        FormScan.Parent = Form.FromHandle(UserInterface.hParent);

                        TWAINState = TWSC.DS_Enabled;
                        if (Convert.ToBoolean(UserInterface.ShowUI))
                        {
                            if (SANE == null)
                            {
                                SANE = new classSANE(SANE_Host, 6566, _dsOrigin);

                                FormScan.SetSANEConnection(SANE);
                                FormScan.SetHostInformation();
                                FormScan.SetFormControls();
                                FormScan.SetUserDefaults();
                            }

                            FormScan.Show();
                        }
                        else
                        {
                            TWAINState = TWSC.DS_Xfer_Ready;

                            CurrentJob.ImagesXferred = 0;
                            Send_TWAIN_Message(TWAINIdentity, AppIdentity, DG.DG_CONTROL, DAT.DAT_NULL, MSG.MSG_XFERREADY, null);
                        }

                        UserInterface.ModalUI = 0;
                        Marshal.StructureToPtr(UserInterface, _pData, true);
                        
                        return TWRC.TWRC_SUCCESS;
                    }
                case MSG.MSG_DISABLEDS:
                    if(TWAINState != TWSC.DS_Enabled)
                        return TWRC.TWRC_FAILURE;
                    else
                    {
                        if (FormScan != null)
                        {
                            FormScan.Hide();
                            FormScan.Parent = null;
                        }
                        TWAINState = TWSC.DS_Opened;
                        return TWRC.TWRC_SUCCESS;
                    }
                default:
                    return TWRC.TWRC_FAILURE;
            }
        }

        public void ButtonScanHandler ()
        {
            TWAINState = TWSC.DS_Xfer_Ready;
            Send_TWAIN_Message(TWAINIdentity, AppIdentity, DG.DG_CONTROL, DAT.DAT_NULL, MSG.MSG_XFERREADY, null);
        }

        private TWRC DG_CONTROL__DAT_SETUPMEMXFER(MSG _MSG, IntPtr _pData)
        {
            TW_SETUPMEMXFER SetupMem;
            SetupMem.MinBufSize = 65536;
            SetupMem.MaxBufSize = 65536;
            SetupMem.Preferred = 65536;

            Marshal.StructureToPtr(SetupMem, _pData, true);
            return TWRC.TWRC_SUCCESS;
        }

        private TWRC DG_CONTROL__DAT_CAPABILITY(MSG _MSG, IntPtr _pData)
        {
            switch(_MSG)
            {
                case MSG.MSG_GET: case MSG.MSG_GETCURRENT: case MSG.MSG_GETDEFAULT:
                    TW_CAPABILITY tw_cap = (TW_CAPABILITY)Marshal.PtrToStructure(_pData, typeof(TW_CAPABILITY));

                    switch (tw_cap.Cap)
                    {
                        case CAP.ICAP_XRESOLUTION:
                        case CAP.ICAP_YRESOLUTION:
                            TW_ONEVALUE_FIX32 onevalue_resolution;
                            onevalue_resolution.ItemType = (ushort)TWTY.TWTY_FIX32;
                            onevalue_resolution.Item = FloatToFIX32(CONTROL_RESOLUTION);
                            IntPtr pContainer_resolution = GlobalAlloc((int)GlobalAllocFlags.GHND, Marshal.SizeOf(onevalue_resolution));
                            tw_cap.ConType = TWON_ONEVALUE;
                            tw_cap.hContainer = pContainer_resolution;
                            Marshal.StructureToPtr(onevalue_resolution, pContainer_resolution, true);
                            Marshal.StructureToPtr(tw_cap, _pData, true);
                            return TWRC.TWRC_SUCCESS;
                        case CAP.ICAP_UNITS:
                            TW_ONEVALUE onevalue_units;
                            onevalue_units.ItemType = (ushort)TWTY.TWTY_UINT16;
                            onevalue_units.Item = TWUN_INCHES;
                            IntPtr pContainer_units = GlobalAlloc((int)GlobalAllocFlags.GHND, Marshal.SizeOf(onevalue_units));
                            tw_cap.ConType = TWON_ONEVALUE;
                            tw_cap.hContainer = pContainer_units;
                            Marshal.StructureToPtr(onevalue_units, pContainer_units, true);
                            Marshal.StructureToPtr(tw_cap, _pData, true);
                            return TWRC.TWRC_SUCCESS;
                        case CAP.ICAP_XFERMECH:
                            TW_ONEVALUE onevalue_xfermech;
                            onevalue_xfermech.ItemType = (ushort)TWTY.TWTY_UINT16;
                            onevalue_xfermech.Item = 0; // NATIVE XFERMECH
                            IntPtr pContainer_xfermech = GlobalAlloc((int)GlobalAllocFlags.GHND, Marshal.SizeOf(onevalue_xfermech));
                            tw_cap.ConType = TWON_ONEVALUE;
                            tw_cap.hContainer = pContainer_xfermech;
                            Marshal.StructureToPtr(onevalue_xfermech, pContainer_xfermech, true);
                            Marshal.StructureToPtr(tw_cap, _pData, true);
                            return TWRC.TWRC_SUCCESS;
                        case CAP.CAP_UICONTROLLABLE:
                        case CAP.CAP_DUPLEXENABLED:
                            TW_ONEVALUE onevalue_ui_dulplex;
                            onevalue_ui_dulplex.ItemType = (ushort)TWTY.TWTY_BOOL;
                            onevalue_ui_dulplex.Item = tw_cap.Cap == CAP.CAP_UICONTROLLABLE ? 1 : CONTROL_DUPLEXENABLED;
                            IntPtr pContainer_ui_duplex = GlobalAlloc((int)GlobalAllocFlags.GHND, Marshal.SizeOf(onevalue_ui_dulplex));
                            tw_cap.ConType = TWON_ONEVALUE;
                            tw_cap.hContainer = pContainer_ui_duplex;
                            Marshal.StructureToPtr(onevalue_ui_dulplex, pContainer_ui_duplex, true);
                            Marshal.StructureToPtr(tw_cap, _pData, true);
                            return TWRC.TWRC_SUCCESS;
                        default:
                            return TWRC.TWRC_FAILURE;
                    }
                case MSG.MSG_SET:
                    return TWRC.TWRC_SUCCESS;
                default:
                    return TWRC.TWRC_FAILURE;
            }
        }

        private TWRC DG_CONTROL__DAT_IDENTITY(IntPtr _pOrigin, MSG _MSG, IntPtr _pData, string _dsOrigin)
        {
            switch(_MSG)
            {
                case MSG.MSG_GET:
                    TWAINIdentity.Id = GetChecksum(_dsOrigin);
                    TWAINIdentity.ProductName = "InSane - " + _dsOrigin;

                    Marshal.StructureToPtr(TWAINIdentity, _pData, true);
                    return TWRC.TWRC_SUCCESS;
                case MSG.MSG_OPENDS:
                    if(TWAINState != TWSC.DSM_Opened)
                        return TWRC.TWRC_FAILURE;
                    else
                    {
                        TWAINIdentity = (TW_IDENTITY)Marshal.PtrToStructure(_pData, typeof(TW_IDENTITY));
                        TW_IDENTITY NewAppIdentity = (TW_IDENTITY)Marshal.PtrToStructure(_pOrigin, typeof(TW_IDENTITY));

                        if (AppIdentity.Id != 0)
                            if (NewAppIdentity.Id != AppIdentity.Id)
                                return TWRC.TWRC_FAILURE;

                        AppIdentity = NewAppIdentity;

                        CurrentJob = new TWAINJob();

                        if (FormScan != null)
                        {
                            FormScan = new formScan();
                            FormScan.btn_scan.Click += this.GUI_ButtonOK_Click;
                            FormScan.FormClosing += this.GUI_FormClosing;
                        }

                        TWAINState = TWSC.DS_Opened;
                        return TWRC.TWRC_SUCCESS;
                    }
                case MSG.MSG_CLOSEDS:
                    if(TWAINState == TWSC.DS_Opened)
                    {
                        if(FormScan != null)
                        {
                            FormScan.GOT_MSG_CLOSEDS = true;
                            FormScan.WindowState = FormWindowState.Minimized;
                            FormScan.Show();
                            FormScan.Close();
                        }

                        if (SANE != null) SANE_Close();

                        AppIdentity = new TW_IDENTITY();
                        TWAINState = TWSC.DSM_Opened;
                    }
                    return TWRC.TWRC_SUCCESS;
                default:
                    return TWRC.TWRC_FAILURE;
            }
        }

        private void SANE_Close()
        {
            SANE.Net_Cancle();
            SANE.Net_Close();
            SANE.Net_Exit();
            SANE = null;
        }

        private void GUI_ButtonOK_Click(object sender, EventArgs e)
        {
            FormScan.SetUserDefaults();
            CurrentJob.ImagesXferred = 0;

            TWAINState = TWSC.DS_Xfer_Ready;
            Send_TWAIN_Message(TWAINIdentity, AppIdentity, DG.DG_CONTROL, DAT.DAT_NULL, MSG.MSG_XFERREADY, default);
        }

        private void GUI_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            if (!FormScan.GOT_MSG_CLOSEDS)
            {
                e.Cancel = true;
                Send_TWAIN_Message(TWAINIdentity, AppIdentity, DG.DG_CONTROL, DAT.DAT_NULL, MSG.MSG_CLOSEDSREQ, default);
            }
        }
    }
}