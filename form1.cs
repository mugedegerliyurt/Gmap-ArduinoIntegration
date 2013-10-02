public partial class Form1 : Form
    {
        GMapOverlay _overlayOne;
        public static String LatLangValues { get; set; }
        BackgroundWorker bw = new BackgroundWorker();
        //Arduino seri portundan data çekmek için gerekli tanımlamalar.
        readonly SerialPort _serialPort = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.None);
        private object _currentSerialSettings;
        

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            bw.DoWork += SerialPortReceive;
            bw.RunWorkerAsync();
            Console.Read();
        }

        private void SerialPortReceive(object sender, DoWorkEventArgs e)
        {
            _serialPort.Open();
            while (true)
            {
                _serialPort.Read();
                //burada read ettikten sonra gelen datayı parametre olarak setoverlay'e atmam gerekmez mi?
                //gelen datayı da LatLangValues 'e set etmem gerekmez mi?
                //o zaman tamamlanmış olmaz mı?
                SetOverlay();
            }
// ReSharper disable once FunctionNeverReturns
        }

        public void SetOverlay()
        {
            gMapControl.Position = new PointLatLng(41.081436, 29.012722);
            gMapControl.MapProvider = GMapProviders.GoogleMap;
            gMapControl.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
            //right click to drag map
            gMapControl.CanDragMap = true;
            gMapControl.MinZoom = 3;
            gMapControl.MaxZoom = 30;
            gMapControl.Zoom = 5;

            gMapControl.Manager.Mode = AccessMode.ServerAndCache;
            //overlay adjust
            _overlayOne = new GMapOverlay(gMapControl, "OverlayOne");
            //marker adjust
            _overlayOne.Markers.Add(new GMap.NET.WindowsForms.Markers.GMapMarkerCross(new PointLatLng(41.081436, 29.012722)));
            //pinning on map
            gMapControl.Overlays.Add(_overlayOne);
        }
    }
}
