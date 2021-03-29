using System;

namespace WavCreator
{
    class WaveCreator
    {
        public double DutyCycle { get; set; } = 0.5;
        private double sinePhase = 0.0;
        private double sawtoothPhase = 0.0;
        private double trianglePhase = 0.0;
        private double pulsePhase = 0.0;

        public double SineWave(double amplitude, double offset, double freq, double samplingHz)
        {
            sinePhase += freq / samplingHz;
            sinePhase -= Math.Floor(sinePhase);
            return Math.Sin(2 * Math.PI * sinePhase) * amplitude + offset;
        }

        public double SawtoothWave(double amplitude, double offset, double freq, double samplingHz)
        {
            sawtoothPhase += freq / samplingHz;
            sawtoothPhase -= Math.Floor(sawtoothPhase);
            return sawtoothPhase * amplitude + offset;
        }

        public double TriangleWave(double amplitude, double offset, double freq, double samplingHz)
        {
            trianglePhase += freq / samplingHz;
            trianglePhase -= Math.Floor(trianglePhase);
            double tr;
            if (trianglePhase > DutyCycle)
            {
                tr = 1.0 - trianglePhase;
            }
            else
            {
                tr = trianglePhase;
            }
            return tr * amplitude + offset;
        }

        public double PulseWave(double amplitude, double offset, double freq, double samplingHz)
        {
            pulsePhase += freq / samplingHz;
            pulsePhase -= Math.Floor(pulsePhase);
            if (pulsePhase > DutyCycle)
            {
                return amplitude + offset;
            }
            else
            {
                return 0.0 + offset;
            }
        }
    }
}
