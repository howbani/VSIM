using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using VSIM.RoadNet.Components;

namespace VSIM.db
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
            string line = "";
            string[] cols;
            string[] vals;

            if (vantComp.ComponentType == ComponentType.RoadSegment)
            {
                cols = new string[]
               {
                    "Pox",
                    "Poy",
                    "Width",
                    "Height",
                    "ComponentType",
                    "RoadOrientation",
                    "LanesCount"

               };
                vals = new string[]
               {

                vantComp.Pox.ToString(),
                vantComp.Poy.ToString(),
                vantComp.Width.ToString(),
                vantComp.Height.ToString(),
                vantComp.ComponentType.ToString(),
                vantComp.RoadOrientation.ToString(),
                vantComp.LanesCount.ToString()
               };

                for (int i = 0; i < cols.Length; i++)
                {
                    line += cols[i] + "=" + vals[i] + ";";
                }
            }
            else if(vantComp.ComponentType== ComponentType.Junction)
            {
                cols = new string[]
              {
                    "Pox",
                    "Poy",
                    "Width",
                    "Height",
                    "ComponentType",
              };
                vals = new string[]
               {

                vantComp.Pox.ToString(),
                vantComp.Poy.ToString(),
                vantComp.Width.ToString(),
                vantComp.Height.ToString(),
                vantComp.ComponentType.ToString(),
               };

                for (int i = 0; i < cols.Length; i++)
                {
                    line += cols[i] + "=" + vals[i] + ";";
                }
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

        /// <summary>
        ///  GET THE NAME OF NETWORKS
        /// </summary>
        /// <returns></returns>
        public static List<string> ImportNetworkNames()
        {
            List<string> NAMES = new List<string>();
            DirectoryInfo d = new DirectoryInfo(dbSettings.PathString);//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.txt"); //Getting Text files
            foreach (FileInfo file in Files)
            {
                NAMES.Add(file.Name);
            }
            return NAMES;
        }


        public static void ImportNetworkNames(ComboBox comboBox)
        {
            
            DirectoryInfo d = new DirectoryInfo(dbSettings.PathString);//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.txt"); //Getting Text files
            foreach (FileInfo file in Files)
            {
                ComboBoxItem comItem = new ComboBoxItem() { Content = file.Name };
                comboBox.Items.Add(comItem);
            }
        }

        /// <summary>
        /// "0Pox","1Poy", "2Width", "3Height","4ComponentType",  "5RoadOrientation", "6LanesCount"
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static VanetComonent GetVanetComonent(string line)
        {
            try
            {
                VanetComonent re = new VanetComonent();
                string[] linecom = line.Split(';');
                re.Pox = Convert.ToDouble(linecom[0].Split('=')[1]);
                re.Poy = Convert.ToDouble(linecom[1].Split('=')[1]);
                re.Width = Convert.ToDouble(linecom[2].Split('=')[1]);
                re.Height = Convert.ToDouble(linecom[3].Split('=')[1]); //
               
                if (linecom[4].Split('=')[1] == "RoadSegment") // TYPE.
                {
                    re.ComponentType = ComponentType.RoadSegment;
                    if (linecom[5].Split('=')[1] == "Vertical")
                    {
                        re.RoadOrientation = RoadOrientation.Vertical;
                    }
                    else
                    {
                        re.RoadOrientation = RoadOrientation.Horizontal;
                    }
                    re.LanesCount = Convert.ToInt16(linecom[6].Split('=')[1]);
                }
                else
                {
                    re.ComponentType = ComponentType.Junction;
                }
                //LanesCount


                return re;
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message + ". For help:" + exp.HelpLink + ".");
            }

            return null; ;
        }


        public static List<VanetComonent> ImportNetwok(string netName)
        {
            List<VanetComonent> re = new List<VanetComonent>();
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
