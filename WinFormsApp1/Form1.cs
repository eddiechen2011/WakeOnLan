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
        // ����Ŀ��������MAC��ַ
        private string targetMacAddress = "70-15-FB-10-17-60"; // �滻ΪĿ��������ʵ��MAC��ַ
        //private static readonly string targetMacAddress = "70-15-FB-10-17-60"; // �滻ΪĿ��������ʵ��MAC��ַ
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

            // ���ö�ʱ��������ÿ������8�㻽�Ѽ����
            DateTime now = DateTime.Now;
            DateTime wakeUpTime = new DateTime(now.Year, now.Month, now.Day, hour, minute, second); // ÿ��8��
            if (wakeUpTime < now)
            {
                wakeUpTime = wakeUpTime.AddDays(1); // �����ǰʱ���ѹ�8�㣬������Ϊ����8��
            }

            TimeSpan interval = wakeUpTime - now;
            wakeUpTimer = new System.Timers.Timer(interval.TotalMilliseconds);
            wakeUpTimer.Elapsed += WakeUpTimer_Elapsed;
            wakeUpTimer.AutoReset = true; // ����Ϊ�Զ��ظ�
            wakeUpTimer.Start();

            listBox1Add("��ʱ���绽������������һ�λ���ʱ��Ϊ��" + wakeUpTime.ToString("yyyy-MM-dd HH:mm:ss"));
            Console.WriteLine("��ʱ���绽������������һ�λ���ʱ��Ϊ��" + wakeUpTime.ToString("yyyy-MM-dd HH:mm:ss"));
            Console.ReadLine(); // ��ֹ����ֱ���˳�
        }
        private void WakeUpTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            listBox1Add("���ڳ��Ի��Ѽ����...");
            Console.WriteLine("���ڳ��Ի��Ѽ����...");
            WakeUpComputer(textBox1.Text);
        }

        private void WakeUpComputer(string macAddress)
        {
            try
            {
                // ��MAC��ַת��Ϊ�ֽ�����
                byte[] macBytes = new byte[6];
                string[] macParts = macAddress.Split('-');
                for (int i = 0; i < 6; i++)
                {
                    macBytes[i] = Convert.ToByte(macParts[i], 16);
                }

                // ����ħ����
                byte[] magicPacket = new byte[102];
                for (int i = 0; i < 6; i++)
                {
                    magicPacket[i] = 0xFF;
                }
                for (int i = 6; i < 102; i += 6)
                {
                    Array.Copy(macBytes, 0, magicPacket, i, 6);
                }

                // ����ħ����
                using (UdpClient client = new UdpClient())
                {
                    client.EnableBroadcast = true;
                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, 9); // �˿ں�ͨ��Ϊ7��9
                    client.Send(magicPacket, magicPacket.Length, endPoint);
                    Console.WriteLine("ħ�����ѷ��ͣ�Ŀ������Ӧ�ѻ��ѡ�");
                    listBox1Add("ħ�����ѷ��ͣ�Ŀ������Ӧ�ѻ��ѡ�");
                }

                resetWakeUpTimer();
                listBox1Add("����WakeUpTimer");
            }
            catch (Exception ex)
            {
                Console.WriteLine("����ʧ�ܣ�" + ex.Message);
                listBox1Add("����ʧ�ܣ�" + ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            textBox1.Text = ConfigRW.appSettings_Read("TargetMacAddress") ?? "";
            dateTimePicker1.Text = ConfigRW.appSettings_Read("WakeUpTime") ?? "08:00:00";

            if (string.IsNullOrEmpty(textBox1.Text))
                MessageBox.Show("��������Ŀ��������MAC��ַ��");
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
            listBox1Add("�ѱ��滽��ʱ�䣺" + dateTimePicker1.Text);
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
            listBox1Add("�ѱ���Ŀ��������MAC��ַ��" + textBox1.Text);
            resetWakeUpTimer();
        }


    }
}
