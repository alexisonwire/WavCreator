using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

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
            double freq = (double)numFreqHz.Value;
            double amplitude = (double)numAmplitude.Value;
            double offset = (double)numOffset.Value;
            uint dataLen = (uint)numDataLength.Value;
            Mode mode = (Mode)cmbMode.SelectedItem;
            var wavFileName = WriteAudio(amplitude, offset, freq, dataLen, mode);
            if (wavFileName != null)
            {
                var player = new System.Media.SoundPlayer(wavFileName);
                player.Play();
            }
        }

        private string WriteAudio(double amplitude, double offset, double freq, uint length, Mode mode)
        {
            string FileName = GetSaveFileName();
            if (FileName == null) { return null; }
            var dataList = new List<double>();
            using (FileStream filStream = new FileStream(FileName, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter binWriter = new BinaryWriter(filStream))
                {
                    WaveHdr Hdr = new WaveHdr();
                    UInt32 DataLength = Hdr.SamplingRate * length;
                    Hdr.NumberOfBytesOfWaveData = Hdr.BlockSize * DataLength;
                    binWriter.Write(Hdr.Bytes);
                    var waveCreator = new WaveCreator();
                    for (UInt32 cnt = 0; cnt < DataLength; cnt++)
                    {
                        double Wave = 0;
                        switch (mode)
                        {
                            case Mode.SineWave:
                                Wave += waveCreator.SineWave(amplitude, offset, freq, Hdr.SamplingRate);
                                break;
                            case Mode.SawtoothWave:
                                Wave += waveCreator.SawtoothWave(amplitude, offset, freq, Hdr.SamplingRate);
                                break;
                            case Mode.TriangleWave:
                                Wave += waveCreator.TriangleWave(amplitude, offset, freq, Hdr.SamplingRate);
                                break;
                            case Mode.PulseWave:
                                Wave += waveCreator.PulseWave(amplitude, offset, freq, Hdr.SamplingRate);
                                break;
                        }
                        dataList.Add(Wave);
                        Int16 Data = (Int16)(Wave * 30000);

                        binWriter.Write(BitConverter.GetBytes(Data));
                        binWriter.Write(BitConverter.GetBytes(Data));
                    }
                }
            }
            string aaa = null;
            chartSampling.Series[0].Points.Clear();
            for (int i = 0; i <= 2000; i++)
            {
                aaa += dataList[i].ToString() + Environment.NewLine;
                chartSampling.Series[0].Points.AddXY(i, dataList[i]);
            }
            return FileName;
        }
    }
}
