public partial class Form1 : Form
    {
        private GMapOverlay _overlayOne;
        public static String LatLangValues { get; set; }
        private readonly BackgroundWorker _bw = new BackgroundWorker();

        //Arduino seri portundan data çekmek için gerekli tanımlamalar.
        private readonly SerialPort _serialPort = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);
        
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

        private void SerialPortReceive(object sender,DoWorkEventArgs e)
        {
                _serialPort.Open();
                
                while (true)
                {
                    // $GPGGA,hhmmss.dd,xxmm.dddd,<N|S>,yyymm.dddd,<E|W>,v,ss,d.d,h.h,M,g.g,M,a.a,xxxx*hh<CR><LF> formatında data Arduinodan karakter karakter gelecek.

                var index = 0;
                var charBuffer = new char[100000];
                    while (true)
                    {
                        var receivedChar = (char) _serialPort.ReadChar();
                        charBuffer[index] = receivedChar;
                        index++;

                        if (receivedChar == 0x0A) // lf'in \n için hex değerini koy
                        {
                            var s = new String(charBuffer, 0, index);
                            //virgüle göre split ettik gelen dataları, böylece her "," de array e bir eleman atıyoruz.
                            String[] stringValues = s.Split(new[] {"LAT"}, StringSplitOptions.RemoveEmptyEntries);

                            string mm = stringValues[stringValues.Length - 1].Replace("\r\n","");

                            string[] value = mm.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            double latValueDouble = Convert.ToDouble(value[0].Replace("=", "").Replace("LON",""));
                            double langValueDouble = Convert.ToDouble(value[1].Replace("=", "").Replace("LON", ""));
                            
                            Invoke((MethodInvoker)(() =>
                            {
                                SetOverlay(latValueDouble, langValueDouble);
                            }));
                            

                            #region old code
                            ////GPGGA lat,lang datasının geldiği sıralama Fastrax için
                            //if (stringValues[0] == "$GPGGA")
                            //{
                            //    //Arduino'dan gelen formatta lat ve lang indekslerinin alanları.
                            //    string latValue = stringValues[3];
                            //    string langValue = stringValues[4];

                            //    double latValueDouble = Convert.ToDouble(latValue);
                            //    double langValueDouble = Convert.ToDouble(langValue);

                            //    index = 0;

                            //}
                            #endregion

                        } 
                    }
                }
            
            // ReSharper disable FunctionNeverReturns
        }

        public void SetOverlay(double serialDataLat,double serialDataLang)
        {
            gMapControl.Position = new PointLatLng(serialDataLat, serialDataLang);
            gMapControl.MapProvider = GMapProviders.BingMap;
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
            //_overlayOne.Markers.Add(new GMap.NET.WindowsForms.Markers.GMapMarkerCross(new PointLatLng(serialDataLat, serialDataLang)));

            //TODO: To change overlay marker image

            var point  = new PointLatLng(serialDataLat,serialDataLang);
            Image image = Image.FromFile(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)+ @"\icon.ico");
            _overlayOne.Markers.Add(new GmapMarker(point,image));

            //pinning on map
            gMapControl.Overlays.Add(_overlayOne);
        }
    }
