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

            //haritayı cachele
            gMapControl.Manager.Mode = AccessMode.ServerAndCache;
            gMapControl.MinZoom = 3;
            gMapControl.MaxZoom = 18;
            gMapControl.Zoom = 15;
        }

        private void SerialPortReceive(object sender,DoWorkEventArgs e)
        {
                _serialPort.Open();
                while (true)
                {
                    // $GPGGA,hhmmss.dd,xxmm.dddd,<N|S>,yyymm.dddd,<E|W>,v,ss,d.d,h.h,M,g.g,M,a.a,xxxx*hh<CR><LF> formatında data Arduinodan karakter karakter gelecek.

                    var index = 0;
                    var charBuffer = new char[100];
                    while (true)
                    {
                        var receivedChar = (char) _serialPort.ReadChar();
                        charBuffer[index] = receivedChar;
                        index++;
                        // lf'in \n için hex değerini koy
                        if (receivedChar == 0x0A) 
                        {
                            var s = new String(charBuffer, 0, index);
                            index = 0;
                            //virgüle göre split ettik gelen dataları, böylece her "," de array e bir eleman atıyoruz.
                            String[] stringValues = s.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                            
                            double latValueDouble = Convert.ToDouble(stringValues[0]);
                            double longValueDouble = Convert.ToDouble(stringValues[1]);

                            if (!IsDisposed)
                            {
                                Invoke((MethodInvoker)(() => SetOverlay(latValueDouble, longValueDouble)));
                            }
                            
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

        public void SetOverlay(double serialDataLat,double serialDataLong)//bunu long'a refactor et
        {
            if (gMapControl.Overlays.Count >= 1) //yanıp sönme olmasın bence olsun ya sanki hareket ediyormuş gbi oluyordu :d havalı :D
                gMapControl.Overlays.RemoveAt(0);

            gMapControl.Position = new PointLatLng(serialDataLat, serialDataLong);//aynısından burada da var? görmedim onu. Bu centerlıyor.
            gMapControl.MapProvider = GMapProviders.BingMap;
            //gMapControl.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
            
            
            
            //gMapControl.Manager.Mode = AccessMode.ServerAndCache;
            //overlay adjust
            _overlayOne = new GMapOverlay(gMapControl, "OverlayOne");
            
            var point  = new PointLatLng(serialDataLat,serialDataLong);
            Image image = Image.FromFile(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)+ @"\icon.ico");
            _overlayOne.Markers.Add(new GmapMarker(point,image));
           
            gMapControl.Overlays.Add(_overlayOne);
        }
    }
