using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavCreator
{
    class WaveCreator
    {
        public double duty { get; set; } = 0.5;
        private double sinePhase = 0.0;
        private double sawtoothPhase = 0.0;
        private double trianglePhase = 0.0;
        private double pulsePhase = 0.0;

        public double sineWave(double amplitude, double freq, double samplingHz)
        {
            sinePhase += freq / samplingHz;
            sinePhase -= Math.Floor(sinePhase);       // 整数部分を引き算
            return Math.Sin(2 * Math.PI * sinePhase) * amplitude;
        }

        public double sawtoothWave(double amplitude, double freq, double samplingHz)
        {
            sawtoothPhase += freq / samplingHz;
            sawtoothPhase -= Math.Floor(sawtoothPhase);       // 整数部分を引き算
            return sawtoothPhase * amplitude;
        }

        public double triangleWave(double amplitude, double freq, double samplingHz)
        {
            trianglePhase += freq / samplingHz;
            trianglePhase -= Math.Floor(trianglePhase);       // 整数部分を引き算
            double tr;
            if (trianglePhase > duty)
            {
                tr = 1.0 - trianglePhase;
            }
            else
            {
                tr = trianglePhase;
            }
            return tr * amplitude;
        }

        public double pulseWave(double amplitude, double freq, double samplingHz)
        {
            pulsePhase += freq / samplingHz;
            pulsePhase -= Math.Floor(pulsePhase);       // 整数部分を引き算
            if (pulsePhase > duty)
            {
                return amplitude;
            }
            else
            {
                return 0.0;
            }
        }
    }
}
