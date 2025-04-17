using System.Net.Sockets;
using System.Net;
using System.Timers;
using System.Windows.Forms;
using System.Configuration;
using eProject;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        // 定义目标计算机的MAC地址
        private string targetMacAddress = "70-15-FB-10-17-60"; // 替换为目标计算机的实际MAC地址
        //private static readonly string targetMacAddress = "70-15-FB-10-17-60"; // 替换为目标计算机的实际MAC地址
        private static System.Timers.Timer wakeUpTimer;

        private bool isFormLoading= true;

        public Form1()
        {
            InitializeComponent();
        }

        private void wake()
        {
            string[] time = dateTimePicker1.Text.Split(":");
            int hour = int.Parse(time[0]);
            int minute = int.Parse(time[1]);
            int second = int.Parse(time[2]);

            // 设置定时任务，例如每天早上8点唤醒计算机
            DateTime now = DateTime.Now;
            DateTime wakeUpTime = new DateTime(now.Year, now.Month, now.Day, hour, minute, second); // 每天8点
            if (wakeUpTime < now)
            {
                wakeUpTime = wakeUpTime.AddDays(1); // 如果当前时间已过8点，则设置为明天8点
            }

            TimeSpan interval = wakeUpTime - now;
            wakeUpTimer = new System.Timers.Timer(interval.TotalMilliseconds);
            wakeUpTimer.Elapsed += WakeUpTimer_Elapsed;
            wakeUpTimer.AutoReset = true; // 设置为自动重复
            wakeUpTimer.Start();

            listBox1Add("定时网络唤醒已启动，下一次唤醒时间为：" + wakeUpTime.ToString("yyyy-MM-dd HH:mm:ss"));
            Console.WriteLine("定时网络唤醒已启动，下一次唤醒时间为：" + wakeUpTime.ToString("yyyy-MM-dd HH:mm:ss"));
            Console.ReadLine(); // 防止程序直接退出
        }
        private void WakeUpTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            listBox1Add("正在尝试唤醒计算机...");
            Console.WriteLine("正在尝试唤醒计算机...");
            WakeUpComputer(textBox1.Text);
        }

        private void WakeUpComputer(string macAddress)
        {
            try
            {
                // 将MAC地址转换为字节数组
                byte[] macBytes = new byte[6];
                string[] macParts = macAddress.Split('-');
                for (int i = 0; i < 6; i++)
                {
                    macBytes[i] = Convert.ToByte(macParts[i], 16);
                }

                // 构造魔术包
                byte[] magicPacket = new byte[102];
                for (int i = 0; i < 6; i++)
                {
                    magicPacket[i] = 0xFF;
                }
                for (int i = 6; i < 102; i += 6)
                {
                    Array.Copy(macBytes, 0, magicPacket, i, 6);
                }

                // 发送魔术包
                using (UdpClient client = new UdpClient())
                {
                    client.EnableBroadcast = true;
                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, 9); // 端口号通常为7或9
                    client.Send(magicPacket, magicPacket.Length, endPoint);
                    Console.WriteLine("魔术包已发送，目标计算机应已唤醒。");
                    listBox1Add("魔术包已发送，目标计算机应已唤醒。");
                }

                resetWakeUpTimer();
                listBox1Add("重置WakeUpTimer");
            }
            catch (Exception ex)
            {
                Console.WriteLine("唤醒失败：" + ex.Message);
                listBox1Add("唤醒失败：" + ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            textBox1.Text = ConfigRW.appSettings_Read("TargetMacAddress") ?? "";
            dateTimePicker1.Text = ConfigRW.appSettings_Read("WakeUpTime") ?? "08:00:00";

            if (string.IsNullOrEmpty(textBox1.Text))
                MessageBox.Show("请先设置目标计算机的MAC地址！");
            else
                wake();

            isFormLoading = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WakeUpComputer(textBox1.Text);
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (isFormLoading)
                return;

            ConfigRW.appSettings_Write("WakeUpTime", dateTimePicker1.Text);
            listBox1Add("已保存唤醒时间：" + dateTimePicker1.Text);
            resetWakeUpTimer();
        }

        private void listBox1Add(string str)
        {
            if (listBox1.InvokeRequired)
            {
                listBox1.Invoke(new Action(() => listBox1.Items.Add(str)));
            }
            else
            {
                listBox1.Items.Add(str);
            }
        }

        private void resetWakeUpTimer()
        {
            wakeUpTimer.Stop();
            wake();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(isFormLoading)
                return;

            ConfigRW.appSettings_Write("TargetMacAddress", textBox1.Text);
            listBox1Add("已保存目标计算机的MAC地址：" + textBox1.Text);
            resetWakeUpTimer();
        }


    }
}
