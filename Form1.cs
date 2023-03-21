using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms;

namespace AppList
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        //部分坐标变量，便于自动排版。
        int labelY = 10;

        private void Form1_Load(object sender, EventArgs e)
        {
            
            //绘制窗口大小
            this.Size = new System.Drawing.Size(455, 531);

            //绘制版本号
            Label labelVer = new Label();
            labelVer.Text = "AppList   Ver 1.0 " ;
            labelVer.AutoSize = true;
            labelVer.Location = new Point(this.ClientSize.Width - labelVer.Width - 20, this.ClientSize.Height - labelVer.Height - 5);
            this.Controls.Add(labelVer);

            //绘制当前登录用户名
            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            Label labelUser = new Label();
            labelUser.Text = userName + " ，您好！";
            labelUser.Location = new Point(10, labelY);
            labelUser.AutoSize = true;
            this.Controls.Add(labelUser);

            labelY += 20;
            List<string> addresses = GetIPv4AddressesWithPrefix("192.");//如有其他需求，可修改搜索。

            // 调试：控制台输出所有符合条件的IP地址
            foreach (string address in addresses)
            {
                Console.WriteLine(address);
            }

            // 循环打印所有IP至from1
            string localaddress = "";
            foreach (string address in addresses)
            {
                localaddress = localaddress + "[" + address + "] ";
            }

            Label labelIP = new Label();
            labelIP.Text = "您的IP为：" + localaddress;
            labelIP.Location = new Point(10, labelY);
            labelIP.AutoSize = true;
            this.Controls.Add(labelIP);

            //文件名，修改时间 等排版。
            labelY += 50;
            Label labelName = new Label();
            labelName.Text = "文件名";
            labelName.Location = new Point(10, labelY);
            labelName.AutoSize = true;
            labelName.Font = new Font("宋体", 9, FontStyle.Bold);
            this.Controls.Add(labelName);

            Label labelDate = new Label();
            labelDate.Text = "修改时间";
            labelDate.Location = new Point(255, labelY);
            labelDate.AutoSize = true;
            labelDate.Font = new Font("宋体", 9, FontStyle.Bold);
            this.Controls.Add(labelDate);

            string url = "http://172.31.218.53/";
            WebClient client = new WebClient();
            byte[] data = client.DownloadData(url);
            string result = Encoding.UTF8.GetString(data);
            string[] lines = result.Split(new[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);
            DrawControls(lines);
        }

        private Dictionary<Button, Timer> buttonTimerDict = new Dictionary<Button, Timer>();

        private void InstallOrUpdate(string fileName, Button button)
        {
            if (!buttonTimerDict.ContainsKey(button))
            {
                // create a timer for the button if it doesn't exist
                Timer timer = new Timer();
                timer.Interval = 30000;
                timer.Tick += (sender, e) =>
                {
                    buttonTimerDict.Remove(button);
                    ((Timer)sender).Stop();
                };
                buttonTimerDict.Add(button, timer);
            }

            Timer buttonTimer = buttonTimerDict[button];

            if (!buttonTimer.Enabled)
            {
                // start the timer and execute the command
                buttonTimer.Tag = button;
                buttonTimer.Start();
                //如有密钥改动，请修改aDMh7PBKZWnBbpuiWds3DQ--，记得等于号改成减号。
                string command = string.Format(@"myrunas /env /user:aDMh7PBKZWnBbpuiWds3DQ--@microsoft.com ""\\ADSERVER\app\{0}""", fileName);
                ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", "/C " + command);
                psi.CreateNoWindow = false;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(psi);
            }
            else
            {
                // show the message box if the button is still in cooldown
                MessageBox.Show("进程运行中...请耐心等待...");
            }   
        }


        private void DrawControls(string[] lines)
        {
            int y = labelY + 20;
            foreach (string line in lines)
            {
                string[] parts = line.Split('|');
                if (parts.Length == 2)
                {
                    Label label1 = new Label();
                    label1.Text = parts[0];
                    label1.Location = new Point(10, y + 10);
                    label1.AutoSize = true;
                    this.Controls.Add(label1);

                    Label label2 = new Label();
                    label2.Text = parts[1];
                    label2.Location = new Point(255, y + 10);
                    label2.AutoSize = true;
                    this.Controls.Add(label2);

                    Button button = new Button();
                    button.Name = "button_" + parts[0];
                    button.Text = "安装/更新";
                    button.Location = new Point(350, y + 5);
                    button.Click += (sender, e) => InstallOrUpdate(parts[0], button);
                    this.Controls.Add(button);

                    // create a timer for each button and add it to the dictionary
                    Timer timer = new Timer();
                    timer.Interval = 30000;
                    timer.Tick += (sender, e) =>
                    {
                        Button btn = (Button)((Timer)sender).Tag;
                        buttonTimerDict.Remove(btn);
                        ((Timer)sender).Stop();
                    };
                    buttonTimerDict.Add(button, timer);

                    y += 30;
                }
            }
        }
        public static List<string> GetIPv4AddressesWithPrefix(string prefix)
        {
            List<string> addresses = new List<string>();

            // 获取所有网络接口
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

            // 枚举每个网络接口
            foreach (NetworkInterface ni in interfaces)
            {
                // 获取接口的IP属性
                IPInterfaceProperties ipProps = ni.GetIPProperties();

                // 枚举接口的所有IP地址
                foreach (UnicastIPAddressInformation addr in ipProps.UnicastAddresses)
                {
                    // 检查IP地址是否以指定前缀开头
                    if (addr.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork &&
                        addr.Address.ToString().StartsWith(prefix))
                    {
                        // 将符合条件的IP地址添加到列表中
                        addresses.Add(addr.Address.ToString());
                    }
                }
            }
            return addresses;
        }
    }
}