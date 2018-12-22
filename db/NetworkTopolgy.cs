using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Windows;
using VANET_SIM.RoadNet.Components;

namespace VANET_SIM.db
{

    public class NetworkTopolgy
    {

        /// <summary>
        /// create new txt file.
        /// </summary>
        /// <param name="NetworkName"></param>
        /// <returns></returns>
        public bool createNewTopology(string NetworkName)
        {
            string pathString = dbSettings.PathString + NetworkName + ".txt";

            if (!File.Exists(pathString))
            {
                FileStream fs1 = new FileStream(pathString, FileMode.CreateNew, FileAccess.Write);
                fs1.Close();
                return true;
            }
            return false;
        }


        public bool SaveVanetComponent(VanetComonent vantComp, string TopologName)
        {
            string pathString = dbSettings.PathString + TopologName + ".txt";
            string[] cols = new string[]
            {
                    "Pox",
                    "Poy",
                    "Width",
                    "Height",
                    "ComponentType",
                    "RoadOrientation",

            };
            string[] vals = new string[]
            {
                
                vantComp.Pox.ToString(),
                vantComp.Poy.ToString(),
                vantComp.Width.ToString(),
                vantComp.Height.ToString(),
                vantComp.ComponentType.ToString(),
                vantComp.RoadOrientation.ToString()
            };
            string line = "";
            for (int i = 0; i < cols.Length; i++)
            {
                line += cols[i] + "=" + vals[i] + ";";
            }

            FileStream fs1 = new FileStream(pathString, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs1);
            sw.WriteLine(line);
            sw.Close();
            fs1.Close();

            return true;
        }

        /// <summary>
        /// get the names of networks.
        /// laod the all the txt files in the topologyies.
        /// </summary>
        /// <returns></returns>
        public static List<NetwokImport> ImportNetworkNames(UiImportTopology ui)
        {
            List<NetwokImport> networks = new List<NetwokImport>();
            DirectoryInfo d = new DirectoryInfo(dbSettings.PathString);//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.txt"); //Getting Text files
            int i = 0;
            foreach (FileInfo file in Files)
            {
                NetwokImport net = new NetwokImport();
                net.UiImportTopology = ui;
                net.lbl_id.Content = i++;
                net.lbl_network_name.Content = file.Name;
                networks.Add(net);
            }
            return networks;
        }


        public static VanetComonent GetVanetComonent(string line)
        {
            try
            {
                VanetComonent re = new VanetComonent();
                string[] linecom = line.Split(';');
                re.Pox = Convert.ToDouble(linecom[0].Split('=')[1]);
                re.Poy = Convert.ToDouble(linecom[1].Split('=')[1]);
                re.Width = Convert.ToDouble(linecom[2].Split('=')[1]);
                re.Height = Convert.ToDouble(linecom[3].Split('=')[1]);
               
                if (linecom[4].Split('=')[1] == "RoadSegment")
                {
                    re.ComponentType = ComponentType.RoadSegment;
                    if (linecom[5].Split('=')[1] == "Vertical")
                    {
                        re.RoadOrientation = RoadOrientation.Vertical;
                    }
                    else re.RoadOrientation = RoadOrientation.Horizontal;
                }
                else
                {
                    re.ComponentType = ComponentType.Junction;
                }


                return re;
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message + ". For help:" + exp.HelpLink + ".");
            }

            return null; ;
        }

        public static List<VanetComonent> ImportNetwok(NetwokImport Netwok)
        {
            List<VanetComonent> re = new List<db.VanetComonent>();
            string netName = Netwok.lbl_network_name.Content.ToString();
            string pathString = dbSettings.PathString + netName;

            string[] lines = File.ReadAllLines(pathString);

            foreach (string lin in lines)
            {
                VanetComonent c= GetVanetComonent(lin);
                re.Add(c);
            }
            return re;
        }
    }
}
