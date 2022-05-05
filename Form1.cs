using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WavCreator
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
            TriangleWave,
            PulseWave
        }

        private string GetSaveFileName()
        {
            var sfd = new SaveFileDialog();
            sfd.FileName = DateTime.Now.ToString("yyyyMMddHHmm_ss") + ".wav";
            sfd.InitialDirectory = Environment.CurrentDirectory;
            sfd.RestoreDirectory = true;
            sfd.OverwritePrompt = true;
            sfd.CheckPathExists = true;
            //sfd.ShowDialog();
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

        public void Delay(List<double> input, List<double> output, double samplingHz, uint cnt)
        {
            int repeat = 2;
            double delayTime = samplingHz * 0.375;
            double a = 0.5;
            if (input.Count == 0) { return; }
            for (int i = 0; i < repeat; i++)
            {
                int m = (int)((double)cnt - (double)i * delayTime);
                //if (input.Count < m) { return; }
                if (m >= 0)
                {
                    var calc = Math.Pow(a, (double)i) * input[m];
                    output[(int)cnt] += calc;
                }
            }
        }

        string wavFileName;

        private void button1_Click_1(object sender, EventArgs e)
        {
            double freq = (double)numFreqHz.Value;
            double amplitude = (double)numAmplitude.Value;
            double offset = (double)numOffset.Value;
            uint dataLen = (uint)numDataLength.Value;
            Mode mode = (Mode)cmbMode.SelectedItem;
            wavFileName = WriteAudio(amplitude, offset, freq, dataLen, mode);
            Task task = Task.Run(() =>
            {
                wavPlay();
            });
        }

        private void wavPlay()
        {
            if (wavFileName != null)
            {
                var player = new System.Media.SoundPlayer(wavFileName);
                player.Play();
            }
        }


        private string WriteAudio(double amplitude, double offset, double freq, uint length, Mode mode)
        {
            string FileName = GetSaveFileName();
            //string FileName = Environment.CurrentDirectory + @"\" + DateTime.Now.ToString("yyyyMMddHHmm_ss") + ".wav";

            if (FileName == null) { return null; }
            var dataList = new List<double>();
            var dataList3 = new List<double>();
            UInt32 samplingNum = 0;
            using (FileStream filStream = new FileStream(FileName, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter binWriter = new BinaryWriter(filStream))
                {
                    WaveHdr Hdr = new WaveHdr();
                    UInt32 DataLength = Hdr.SamplingRate * length;
                    samplingNum = DataLength;
                    Hdr.NumberOfBytesOfWaveData = Hdr.BlockSize * DataLength;
                    binWriter.Write(Hdr.Bytes);
                    var waveCreator = new WaveCreator();

                    var tmpfreq = freq;

                    for (UInt32 cnt = 0; cnt < DataLength; cnt++)
                    {
                        var aaa = new Random();
                        var rand = aaa.Next(1, 5);

                        //freq = tmpfreq * rand;
                        double Wave = 0;
                        switch (mode)
                        {
                            case Mode.SineWave:
                                Wave += waveCreator.SineWave(amplitude, freq, Hdr.SamplingRate);
                                break;
                            case Mode.SawtoothWave:
                                Wave += waveCreator.SawtoothWave(amplitude, freq, Hdr.SamplingRate);
                                break;
                            case Mode.TriangleWave:
                                Wave += waveCreator.TriangleWave(amplitude, freq, Hdr.SamplingRate);
                                break;
                            case Mode.PulseWave:
                                Wave += waveCreator.PulseWave(amplitude, freq, Hdr.SamplingRate);

                                break;
                        }
                        Wave += offset;
                        dataList.Add(Wave);
                    }

                    foreach (var item in dataList)
                    {
                        Int16 Data = (Int16)(item * 30000);
                        binWriter.Write(BitConverter.GetBytes(Data));
                        binWriter.Write(BitConverter.GetBytes(Data));
                    }
                    var dataList2 = new List<double>(dataList);

                    //uint ucnt = 0;
                    //foreach (var item in dataList)
                    //{
                    //    Delay(dataList, dataList2, Hdr.SamplingRate, ucnt);
                    //    ucnt++;
                    //}

                    foreach (var item in dataList2)
                    {
                        Int16 Data = (Int16)(item * 30000);
                        binWriter.Write(BitConverter.GetBytes(Data));
                        binWriter.Write(BitConverter.GetBytes(Data));
                    }
                    dataList3 = dataList2;
                }
                chartSampling.Series[0].Points.Clear();

                for (int i = 0; i <= samplingNum / length; i++)
                //for (int i = 0; i < dataList.Count; i++)
                {
                    chartSampling.Series[0].Points.AddXY(i, dataList[i]);
                    //Debug.WriteLine(dataList[i]);
                }
            }
            return FileName;
        }
    }
}
