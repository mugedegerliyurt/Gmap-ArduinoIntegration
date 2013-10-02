public partial class Form1 : Form
    {
        GMapOverlay _overlayOne;
        public static String LatLangValues { get; set; }
        readonly BackgroundWorker _bw = new BackgroundWorker();
        //Arduino seri portundan data çekmek için gerekli tanımlamalar.
        readonly SerialPort _serialPort = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.None);


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            _bw.DoWork += SerialPortReceive;
            _bw.RunWorkerAsync();
            Console.Read();
        }

        private void SerialPortReceive(object sender, DoWorkEventArgs e)
        {
            _serialPort.Open();

            while (true)
            {
                // $GPGGA,hhmmss.dd,xxmm.dddd,<N|S>,yyymm.dddd,<E|W>,v,ss,d.d,h.h,M,g.g,M,a.a,xxxx*hh<CR><LF> formatında data Arduinodan karakter karakter gelecek.
                
                char[] charBufferValue = {};

                int index = 0;
                var charBuffer = new char[200];

                while (true)
                {
                    var receivedChar = (char)_serialPort.ReadChar();
                    charBuffer[index] = receivedChar;
                    index++;
                    if (receivedChar == 0x10) // lf'in hex değerini koy
                    {
                        var s = new String(charBufferValue, 0, index);
                        String[] stringValues = s.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        index = 0;
                    }
                }


                //double latitude = charBufferValue;
                //double langtitude = charBufferValue;

                //SetOverlay(latitude, langtitude);
            }
        // ReSharper disable FunctionNeverReturns
        }

        public void SetOverlay(double serialDataLat,double serialDataLang)
        {
            gMapControl.Position = new PointLatLng(serialDataLat, serialDataLang);
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
            _overlayOne.Markers.Add(new GMap.NET.WindowsForms.Markers.GMapMarkerCross(new PointLatLng(serialDataLat, serialDataLang)));
            //pinning on map
            gMapControl.Overlays.Add(_overlayOne);
        }
    }
