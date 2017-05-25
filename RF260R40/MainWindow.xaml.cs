using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RF260R40
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //串口实例化
        SerialPort com = new SerialPort("COM4", 19200, Parity.Odd, 8, StopBits.One);
        //SET-RS232.....02 3041 3030 3030 3030 3035 3030 3030 3030 3031 03

        // 设置RS232 目前调试不好使  zy 
        Byte[] btSETRS232 = { 0x02, 0x30, 0x41, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x35, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x31, 0x03 };

        //故障复位......     02    30    41    30    30    30    30    30    30    38    35    30    32    30    30    30    30    30    31    30    30    30    31    03
        // 故障复位   zy     02    30    41    30    30    30    30    30    30    38    35    30    32    30    30    30    30    30    31    30    30    30    31    03 
        Byte[] btRESET = { 0x02, 0x30, 0x41, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x38, 0x35, 0x30, 0x32, 0x30, 0x30, 0x30, 0x30, 0x30, 0x31, 0x30, 0x30, 0x30, 0x31, 0x03 };

        //初始化........    02    30    41    30    30    30    30    30    30    30    35    30    30    30    30    30    30    30    31    30    30    30    31    03
        // 初始化  zy       02    30    41    30    30    30    30    30    30    30    35    30    30    30    30    30    30    30    31    30    30    30    31    03
        Byte[] btINIT = { 0x02, 0x30, 0x41, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x35, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x31, 0x30, 0x30, 0x30, 0x31, 0x03 };

        //“模式-0”....02 3036 3034 3030 3030 3030 3030 3030 03
        // 设置模式-0  zy       02    30    36    30    34    30    30    30    30    30    30    30    30    30    30    03
        Byte[] btModeOne = { 0x02, 0x30, 0x36, 0x30, 0x34, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x03 };

        //读指令........02 3035 3032 3030 3030 3030 4638 03//读全部
        //读指令........02 3035 3032 3030 3030 3030 3038 03//读全部
        Byte[] btRead = { 0x02, 0x30, 0x35, 0x30, 0x32, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x46, 0x38, 0x03 };

        // zy 读取指令 增加设置读多少数据
        Byte[] btRead1;

        //写指令......  .    02    3044        3031        3030       3030         3030     4638 ______ 03
        Byte[] btWrite;

        public MainWindow()
        {
            com.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            com.Open();//打开串口

            RF260R40Init();
            InitializeComponent();

        }
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            string indata = com.ReadExisting();
            this.tb_ReadText.Dispatcher.Invoke(new Action(delegate { this.tb_ReadText.Text += indata; }));
        }
        //RFID初始化
        private void RF260R40Init()
        {
            try
            {
                ////SET-RS232
                //com.Write(btSETRS232, 0, btSETRS232.Length);
                //MessageBox.Show(com.ReadExisting());
                ////故障复位
                //com.Write(btRESET, 0, btRESET.Length);
                //MessageBox.Show(com.ReadExisting());
                ////初始化
                //com.Write(btINIT, 0, btINIT.Length);
                //MessageBox.Show(com.ReadExisting());
                //////“模式-0”
                ////com.Write(btModeOne, 0, btModeOne.Length);
                ////MessageBox.Show(com.ReadExisting());
            }
            catch (System.Exception error)
            {
                MessageBox.Show(error.Message, "RF260R40-初始化", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //写入按钮
        private void btn_Write(object sender, RoutedEventArgs e)
        {
            try
            {
                //获取界面数据
                string writeString = this.tb_WriteText.Text;
                List<byte> btWrite0 = new List<byte>();

                // zy 获取界面写入数据位数
                string WriteLength = this.tb_WriteLength.Text;
                int Writelen = int.Parse(WriteLength);

                //获取输入字符串长度
                int num = writeString.Length;

                // zy 判断输入字符串长度和数据位数是否一样
                if (num == Writelen)
                {
                    //判断字符串长度
                    if (num > 0 && num < 248)
                    {
                        //转换界面数据
                        byte[] byteArray = System.Text.Encoding.ASCII.GetBytes(writeString);
                        int n = byteArray.Length;
                        if (n % 2 == 1)//如果为奇数
                        {
                            n++;
                            n = n / 2;
                            //净字节长度
                            int m = n + 5;
                            string strm = String.Format("{0:X}", m);
                            if (strm.Length == 1)
                                strm = "0" + strm;
                            string strn = String.Format("{0:X}", n);
                            if (strn.Length == 1)
                                strn = "0" + strn;

                            // zy  当输入为奇数时，不能补空格，补空格  当前设置 补0 
                            writeString = strm + "01000000" + strn + writeString + "0";
                            //编写指令
                            btWrite0.Add(0x02);
                            byte[] bt001 = System.Text.Encoding.ASCII.GetBytes(writeString);
                            for (int i = 0; i < bt001.Length; i++)
                                btWrite0.Add(bt001[i]);
                            btWrite0.Add(0x03);
                        }
                        else//如果为偶数
                        {
                            n = n / 2;
                            //净字节长度
                            int m = n + 5;
                            string strm = String.Format("{0:X}", m);
                            if (strm.Length == 1)
                                strm = "0" + strm;
                            string strn = String.Format("{0:X}", n);
                            if (strn.Length == 1)
                                strn = "0" + strn;
                            writeString = strm + "01000000" + strn + writeString;
                            //编写指令
                            btWrite0.Add(0x02);
                            byte[] bt001 = System.Text.Encoding.ASCII.GetBytes(writeString);
                            for (int i = 0; i < bt001.Length; i++)
                                btWrite0.Add(bt001[i]);
                            btWrite0.Add(0x03);
                        }
                        //格式转换
                        btWrite = btWrite0.ToArray();
                    }
                    else
                    {
                        MessageBox.Show("写入错误！数据为空或者超过248字节，请检查输入数据！");
                    }
                    //写数据........02 3044 3031 3030 3030 3030 4638 ______ 03
                    com.Write(btWrite, 0, btWrite.Length);
                    //写确认........02 3042 3031 3030 03
                    //读数据
                    //界面显示数据
                    //this.tb_ReadText.Text = null;
                    //com.Write(btRead, 0, btRead.Length);
                }
                else
                {
                    MessageBox.Show("写入错误！数据长度与数据位数不一致，请检查输入数据！");
                }
            }
            catch (System.Exception error)
            {
                MessageBox.Show(error.Message, "RF260R40 - 写入", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //读取按钮
        private void btn_Read(object sender, RoutedEventArgs e)
        {
            try
            {
                //读全部........02 3035 3032 3030 3030 3030 4638 03
                this.tb_ReadText.Text = null;
                List<byte> btRead0 = new List<byte>();

                // zy 用46 38 读全部 会报错 增加设置读取位数

                // zy 获取界面写入数据位数
                string ReadLength = this.tb_ReadLength.Text;
                int Readlen = int.Parse(ReadLength);
                // zy 数据是由高低位组成 数据的长度是数据位数的一半
                Readlen = Readlen / 2;
                string Rlen = String.Format("{0:X}", Readlen);
                if (Rlen.Length == 1)
                    Rlen = "0" + Rlen;

                string Readstring = "0502000000" + Rlen;

                btRead0.Add(0x02);
                byte[] bt001 = System.Text.Encoding.ASCII.GetBytes(Readstring);
                for (int i = 0; i < bt001.Length; i++)
                    btRead0.Add(bt001[i]);
                btRead0.Add(0x03);

                btRead1 = btRead0.ToArray();

                com.Write(btRead1, 0, btRead1.Length);
                //System.Threading.Thread.Sleep(500);
                //MessageBox.Show(com.ReadExisting());
                //读确认........02 3044 3032 3030 3030 3030 4638 ______ 03
                //界面显示数据
            }
            catch (System.Exception error)
            {
                MessageBox.Show(error.Message, "RF260R40 - 读取", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //初始化
        private void btn_Init(object sender, RoutedEventArgs e)
        {
            this.tb_ReadText.Text = null;
            //初始化
            com.Write(btINIT, 0, btINIT.Length);
            //MessageBox.Show(com.ReadExisting());
        }
        //故障复位
        private void btn_Reset(object sender, RoutedEventArgs e)
        {
            this.tb_ReadText.Text = null;
            //故障复位
            com.Write(btRESET, 0, btRESET.Length);

            //MessageBox.Show(com.ReadExisting());
        }
        //设置
        private void btn_Set(object sender, RoutedEventArgs e)
        {
            this.tb_ReadText.Text = null;
            //SET-RS232
            com.Write(btSETRS232, 0, btSETRS232.Length);
            //MessageBox.Show(com.ReadExisting());
        }
    }
}
