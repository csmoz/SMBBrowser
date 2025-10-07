using SMBLibrary;
using SMBLibrary.Client;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace SMBBrowser
{
    public partial class Form1 : Form
    {
        private SMB2Client? m_client;
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();

            m_client = new SMB2Client();

            IPAddress ip;
            if (!IPAddress.TryParse(textBoxIP.Text, out ip))
            {
                Log("Enter a valid IP address.", Color.Red);
                return;
            }
            int port;
            if (!int.TryParse(textBoxPort.Text, out port) || port > 65535)
            {
                Log("Enter a valid Port number.", Color.Red);
                return;
            }

            if (IsPortOpen(ip.ToString(), port, TimeSpan.FromSeconds(5)))
            {
                Log("Port is open", Color.Green);
            }
            else
            {
                Log("Port is closed", Color.Red);
                return;
            }

            Log($"Attempting to connect {ip}:{port}", richTextBoxLog.ForeColor);
            bool isConnected = CustomConnect(ip, port);
            if (!isConnected)
            {
                Log("Failed to connect to server. Check if SMB daemon service is running.", Color.Red);
                return;
            }
            Log("Successfully connected.", Color.Green);

            string domain = Environment.UserDomainName;
            string username = textBoxUsername.Text;
            string password = textBoxPassword.Text;

            Log("Attempting to login...", richTextBoxLog.ForeColor);
            NTStatus loginStatus = m_client.Login(domain, username, password);
            if (loginStatus != NTStatus.STATUS_SUCCESS)
            {
                Log("Failed to login. Make sure you enter target computer credentials not yours. Check if you have permissions for this server.", Color.Red);
                return;
            }
            Log("Successfully logged in.", Color.Green);

            Log("Listing shares...", richTextBoxLog.ForeColor);
            NTStatus listStatus;
            List<string> shares = m_client.ListShares(out listStatus);
            if (listStatus != NTStatus.STATUS_SUCCESS)
            {
                Log("Failed to list shares.", Color.Red);
                Log($"NTStatus code: {listStatus.ToString()}", Color.OrangeRed);
                return;
            }
            Log("Status: OK", Color.Green);
            Log("- - - -", Color.Black);

            foreach (string share in shares)
            {
                NTStatus treeStatus;
                ISMBFileStore fileStore = m_client.TreeConnect(share, out treeStatus);
                if (treeStatus == NTStatus.STATUS_SUCCESS)
                {
                    TreeNode root = new TreeNode(share) { Tag = share };
                    root.Nodes.Add("Loading...");
                    treeView1.Nodes.Add(root);
                    fileStore.Disconnect();
                }
            }
        }

        bool IsPortOpen(string host, int port, TimeSpan timeout)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    var result = client.BeginConnect(host, port, null, null);
                    var success = result.AsyncWaitHandle.WaitOne(timeout);
                    client.EndConnect(result);
                    return success;
                }
            }
            catch
            {
                return false;
            }
        }

        private bool CustomConnect(IPAddress ip, int port)
        {
            SMBTransportType transport = SMBTransportType.DirectTCPTransport;

            MethodInfo connectMethod = typeof(SMB2Client).GetMethod(
                "Connect",
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new Type[] { typeof(IPAddress), typeof(SMBTransportType), typeof(int), typeof(int) },
                null
                );

            if (connectMethod == null)
            {
                throw new Exception("Could not find method via reflection.");
            }

            return (bool)connectMethod.Invoke(m_client, new object[] { ip, transport, port, 2000 });
        }

        private void Log(string message, Color color)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            int start = richTextBoxLog.Text.Length;
            richTextBoxLog.AppendText(message + Environment.NewLine);
            int end = richTextBoxLog.Text.Length;

            richTextBoxLog.Select(start, end - start);
            richTextBoxLog.SelectionColor = color;

            richTextBoxLog.SelectionStart = richTextBoxLog.TextLength;
            richTextBoxLog.SelectionColor = richTextBoxLog.ForeColor;

            richTextBoxLog.ScrollToCaret();
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Text == "Loading...")
            {
                e.Node.Nodes.Clear();

                // Find the root share node
                TreeNode rootNode = e.Node;
                while (rootNode.Parent != null)
                {
                    rootNode = rootNode.Parent;
                }
                string shareName = rootNode.Text;

                // Build path relative to share
                string path = GetRelativePath(e.Node);

                // Connect to the share (only for the root node!)
                NTStatus treeStatus;
                var store = m_client.TreeConnect(shareName, out treeStatus);
                if (treeStatus != NTStatus.STATUS_SUCCESS)
                {
                    Log($"Failed to connect to share {shareName}: {treeStatus}", Color.Red);
                    return;
                }

                // Populate the folder contents
                PopulateNode(store, e.Node, path);

                store.Disconnect(); // close share after use
            }
        }

        private string GetRelativePath(TreeNode node)
        {
            List<string> parts = new List<string>();
            TreeNode current = node;
            while (current.Parent != null)
            {
                parts.Insert(0, current.Text);
                current = current.Parent;
            }
            return string.Join("\\", parts);
        }

        private void PopulateNode(ISMBFileStore store, TreeNode node, string path)
        {
            object dirHandle = null;

            try
            {
                NTStatus status;
                FileStatus fileStatus;

                status = store.CreateFile(
                    out dirHandle,
                    out fileStatus,
                    path,
                    AccessMask.GENERIC_READ,
                    SMBLibrary.FileAttributes.Directory,
                    ShareAccess.Read | ShareAccess.Write,
                    CreateDisposition.FILE_OPEN,
                    CreateOptions.FILE_DIRECTORY_FILE,
                    null
                    );
                if (status != NTStatus.STATUS_SUCCESS)
                {
                    Log($"Failed to create file: {status}", Color.Red);
                    return;
                }

                List<QueryDirectoryFileInformation> fileList;
                status = store.QueryDirectory(out fileList, dirHandle, "*", FileInformationClass.FileDirectoryInformation);

                foreach (var f in fileList)
                {
                    string name = null;
                    bool isDir = false;

                    // Determine concrete type
                    if (f is FileDirectoryInformation dirInfo)
                    {
                        name = dirInfo.FileName;
                        isDir = (dirInfo.FileAttributes & SMBLibrary.FileAttributes.Directory) != 0;
                    }
                    else if (f is FileFullDirectoryInformation fullInfo)
                    {
                        name = fullInfo.FileName;
                        isDir = (fullInfo.FileAttributes & SMBLibrary.FileAttributes.Directory) != 0;
                    }
                    else if (f is FileBothDirectoryInformation bothInfo)
                    {
                        name = bothInfo.FileName;
                        isDir = (bothInfo.FileAttributes & SMBLibrary.FileAttributes.Directory) != 0;
                    }
                    else if (f is FileNamesInformation namesInfo)
                    {
                        name = namesInfo.FileName;
                        isDir = false; // FileNamesInformation doesn’t include attributes
                    }

                    if (string.IsNullOrEmpty(name) || name == "." || name == "..") continue;

                    TreeNode child = new TreeNode(name)
                    {
                        Tag = string.IsNullOrEmpty(path) ? name : path + "\\" + name
                    };

                    if (isDir)
                    {
                        child.Nodes.Add("Loading..."); // lazy load
                    }

                    node.Nodes.Add(child);
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message, Color.Red);
            }
            finally
            {
                if (dirHandle != null)
                {
                    store.CloseFile(dirHandle);
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_client != null)
            {
                m_client.Disconnect();
            }
        }
    }
}
