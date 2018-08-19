using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace joselima.dicom.network {
    public static class CEchoScp {
        public static string StartListening(Int32 port, string AETitle) {

            TcpListener server = null;
            try {
                // Set the TcpListener on port 13000.
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress localAddr = ipHostInfo.AddressList.SingleOrDefault(x => x.AddressFamily.Equals(AddressFamily.InterNetwork));
                //IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                var buffer = new Byte[256];
                String dataText = null;

                // Enter the listening loop.
                while (true) {
                    Console.Write($"Waiting for a connection on {localAddr.ToString()}:{port}...");

                    // Perform a blocking call to accept requests.
                    // You could also user server.AcceptSocket() here.
                    using (TcpClient client = server.AcceptTcpClient()) {

                        Console.WriteLine($"Connected to {client.Client.RemoteEndPoint.ToString()}");

                        dataText = null;
                        //var echoRqFilePath = "echo_rq.dat";
                        //File.Create(echoRqFilePath);
                        //using (var logStream = new BinaryWriter(File.OpenWrite(echoRqFilePath))) {


                        // Get a stream object for reading and writing
                        using (NetworkStream stream = client.GetStream()) {
                            int readLen;

                            // Loop to receive all the data sent by the client.
                            while ((readLen = stream.Read(buffer, 0, buffer.Length)) != 0) {

                                //logStream.Write(buffer, 0, readLen);
                                AssociateRequest parsedRequest = CEchoParser.ParseRequest(buffer, readLen);
                                //Console.WriteLine("C-Echo request received:");
                                //foreach (var att in parsedRequest) {
                                //    Console.WriteLine(att.Value.ToString());
                                //}

                                // Translate data bytes to a ASCII string.
                                //dataText = Encoding.ASCII.GetString(buffer, 0, readLen);
                                //Console.WriteLine("Received: {0}", dataText);

                                // Process the data sent by the client.
                                //dataText = dataText.ToUpper();

                                //byte[] msg = Encoding.ASCII.GetBytes(dataText);
                                var responseMsg = buffer;

                                // Send back a response.
                                stream.Write(responseMsg, 0, responseMsg.Length);
                                Console.WriteLine("Sent: {0}", dataText);
                            }
                            //}

                        }

                        // Shutdown and end connection
                        client.Close();
                    }

                }
            }
            catch (SocketException e) {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally {
                // Stop listening for new clients.
                server.Stop();
            }


            return "";
        }


        public static void StartClient() {
            // Data buffer for incoming data.  
            byte[] bytes = new byte[1024];

            // Connect to a remote device.  
            try {
                // Establish the remote endpoint for the socket.  
                // This example uses port 11000 on the local computer.  
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                // Create a TCP/IP  socket.  
                Socket sender = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.  
                try {
                    sender.Connect(remoteEP);

                    Console.WriteLine("Socket connected to {0}",
                        sender.RemoteEndPoint.ToString());

                    // Encode the data string into a byte array.  
                    byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");

                    // Send the data through the socket.  
                    int bytesSent = sender.Send(msg);

                    // Receive the response from the remote device.  
                    int bytesRec = sender.Receive(bytes);
                    Console.WriteLine("Echoed test = {0}",
                        Encoding.ASCII.GetString(bytes, 0, bytesRec));

                    // Release the socket.  
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane) {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se) {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e) {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
