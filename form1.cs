 public partial class Form1 : Form
    {
        GMapOverlay _overlayOne;
        public static String LatLangValues { get; set; }

        //Arduino seri portundan data çekmek için gerekli tanımlamalar.
        readonly SerialPort _serialPort = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.None);
        private object _currentSerialSettings;
        BackgroundWorker bw = new BackgroundWorker();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            //Threads work asyncron so order is not important.
            //To get serial data from Arduino
            //Thread tid1 = new Thread(GetSerialDataThread);
            //To set overlays on map from serial data from Arduino
            //Thread tid2 = new Thread(SetOverlayFromSerialDataThread);
            
            //tid1.Start();
            //tid2.Start();
            
            bw.DoWork += new DoWorkEventHandler(SerialPortReceive);
            bw.RunWorkerAsync();
            Console.Read();
        }

        private void SerialPortReceive()
        {
          _serialPort.Open();
          while(true)
          {
            _serialPort.Read();
           
           
           
          }
          
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

    }
