 public partial class Form1 : Form
    {
        GMapOverlay _overlayOne;
        public static String LatLangValues { get; set; }
        delegate void BackgroundWorker(string text);

        //Arduino seri portundan data çekmek için gerekli tanımlamalar.
        readonly SerialPort _serialPort = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);
        private object _currentSerialSettings;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            //Threads work asyncron so order is not important.
            //To get serial data from Arduino
            Thread tid1 = new Thread(GetSerialDataThread);
            //To set overlays on map from serial data from Arduino
            Thread tid2 = new Thread(SetOverlayFromSerialDataThread);
            
            tid1.Start();
            tid2.Start();
        }

        private void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SetText(_serialPort.ReadExisting());
            }

            catch (Exception ex)
            {
                SetText(ex.ToString());
            }
            //Write the serial port data to the console.
            Console.Write(_serialPort.ReadExisting());
        }

        public void GetSerialDataThread()
        {
            //Set the datareceived event handler
            _serialPort.DataReceived += sp_DataReceived;
            //Open the serial port
            _serialPort.Open();
            //Read from the console, to stop it from closing.
            Console.Read();
        }

        public void SetOverlayFromSerialDataThread()
        {
            gMapControl.Position = new PointLatLng(41.081436, 29.012722);
            gMapControl.MapProvider = GMapProviders.BingMap;
            gMapControl.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
            //right click to drag map
            gMapControl.CanDragMap = true;
            gMapControl.MinZoom = 3;
            gMapControl.MaxZoom = 30;
            gMapControl.Zoom = 5;

            //GMaps.Instance.Mode = AccessMode.CacheOnly;

            gMapControl.Manager.Mode = AccessMode.ServerAndCache;
            //overlay adjust
            _overlayOne = new GMapOverlay(gMapControl, "OverlayOne");
            //marker adjust
            _overlayOne.Markers.Add(new GMap.NET.WindowsForms.Markers.GMapMarkerCross(new PointLatLng(41.081436, 29.012722)));
            //pinning on map
            gMapControl.Overlays.Add(_overlayOne);

        }

        private void SetText(string text)
        {
            //if (this.txtOutput.InvokeRequired)
            {
                BackgroundWorker d = SetText;
                BeginInvoke(d, new object[] { text });
            }
            //else
            {
               // txtOutput.AppendText(text);
            }
        }


    }
