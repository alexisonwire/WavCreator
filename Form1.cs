using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace WavCreater
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            SetComb();
        }

        public enum Mode
        {
            SineWave,
            SawtoothWave,
            Step
        }

        private string GetSaveFileName()
        {
            var sfd = new SaveFileDialog();
            sfd.FileName = DateTime.Now.ToString("yyyyMMddHHmm_ss") + ".wav";
            sfd.InitialDirectory = Environment.CurrentDirectory;
            sfd.RestoreDirectory = true;
            sfd.OverwritePrompt = true;
            sfd.CheckPathExists = true;
            sfd.ShowDialog();
            return sfd.FileName;
        }

        private void SetComb()
        {
            cmbMode.Items.Clear();
            foreach (Mode x in Enum.GetValues(typeof(Mode)))
            {
                cmbMode.Items.Add(x);
            }
            cmbMode.SelectedIndex = 0;
        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            WriteAudio((uint)numDataLength.Value);
        }

        private double SawtoothWaveCreate(uint cnt, double amplitude, double refFreq, double samplingHz)
        {
            double data = 0f;
            for (int i = 1; i <= 15; i++) /* 15倍音までの重ね合わせ */
            {
                data += amplitude / i * Math.Sin(2.0 * Math.PI * refFreq * i * cnt / samplingHz);
            }
            return data;
        }

        private double PulseWaveCreate(uint cnt, double amplitude, double refFreq, double samplingHz)
        {
            double data = 0f;
            for (int i = 1; i <= 15; i++) /* 15倍音までの重ね合わせ */
            {
                if (i % 2 == 1) /* 奇数次の倍音のみ重ね合わせる */
                {
                    data += amplitude / i * Math.Sin(2.0 * Math.PI * refFreq * i * cnt / samplingHz);
                }
            }
            return data;
        }


        private void WriteAudio(uint length)
        {
            string FileName = GetSaveFileName();
            var dataList = new List<double>();

            using (FileStream filStream = new FileStream(FileName, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter binWriter = new BinaryWriter(filStream))
                {
                    WaveHdr Hdr = new WaveHdr();
                    UInt32 DataLength = Hdr.SamplingRate * length;
                    Hdr.NumberOfBytesOfWaveData = Hdr.BlockSize * DataLength;
                    binWriter.Write(Hdr.Bytes);
                    for (UInt32 cnt = 0; cnt < DataLength; cnt++)
                    {
                        double Radian = (double)cnt / Hdr.SamplingRate;
                        Radian *= 2 * Math.PI;

                        double Wave = 0;
                        double amplitude = 5;
                        double refFreq = 250;
                        //Wave += Math.Sin(Radian * 1336);
                        //Wave += Math.Sin(Radian * 941);

                        //Wave /= 2;

                        Wave += SawtoothWaveCreate(cnt, amplitude, refFreq, Hdr.SamplingRate);
                        Wave += PulseWaveCreate(cnt, amplitude, refFreq, Hdr.SamplingRate);
                        //Debug.WriteLine(Wave);
                        dataList.Add(Wave);
                        Int16 Data = (Int16)(Wave * 30000);

                        binWriter.Write(BitConverter.GetBytes(Data));
                        binWriter.Write(BitConverter.GetBytes(Data));
                    }
                }
            }
            string aaa = null;
            for (int i = 0; i <= 1000; i++) 
            {
                aaa += dataList[i].ToString() + Environment.NewLine;
                chartSampling.Series[0].Points.AddXY(i, dataList[i]);
            }
            ;
        }
    }
}
