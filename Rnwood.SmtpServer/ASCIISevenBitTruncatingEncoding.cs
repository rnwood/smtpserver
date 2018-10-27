namespace Rnwood.SmtpServer
{
    using System.Linq;
    using System.Text;

    public class ASCIISevenBitTruncatingEncoding : Encoding
    {
        public ASCIISevenBitTruncatingEncoding()
        {
            this.asciiEncoding = Encoding.GetEncoding("ASCII", new EncodingFallback(),
                                                  new DecodingFallback());
        }

        private Encoding asciiEncoding;

        public override int GetByteCount(char[] chars, int index, int count)
        {
            return this.asciiEncoding.GetByteCount(chars, index, count);
        }

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            return this.asciiEncoding.GetBytes(chars, charIndex, charCount, bytes, byteIndex);
        }

        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            return this.asciiEncoding.GetCharCount(bytes, index, count);
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            return this.asciiEncoding.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
        }

        public override int GetMaxByteCount(int charCount)
        {
            return this.asciiEncoding.GetMaxByteCount(charCount);
        }

        public override int GetMaxCharCount(int byteCount)
        {
            return this.asciiEncoding.GetMaxCharCount(byteCount);
        }

        private class EncodingFallback : EncoderFallback
        {
            public override int MaxCharCount => 1;

            public override EncoderFallbackBuffer CreateFallbackBuffer()
            {
                return new Buffer();
            }

            private class Buffer : EncoderFallbackBuffer
            {
                public override bool Fallback(char charUnknown, int index)
                {
                    this.@char = this.FallbackChar(charUnknown);
                    this.charRead = false;
                    return true;
                }

                public override bool Fallback(char charUnknownHigh, char charUnknownLow, int index)
                {
                    this.@char = this.FallbackChar(charUnknownLow);
                    this.charRead = false;
                    return true;
                }

                private char FallbackChar(char charUnknown)
                {
                    return (char)(charUnknown & 127);
                }

                public override char GetNextChar()
                {
                    if (!this.charRead)
                    {
                        this.charRead = true;
                        return this.@char;
                    }

                    return '\0';
                }

                public override bool MovePrevious()
                {
                    if (this.charRead)
                    {
                        this.charRead = false;
                        return true;
                    }

                    return false;
                }

                private char @char;
                private bool charRead;

                public override int Remaining => !this.charRead ? 1 : 0;
            }
        }

        private class DecodingFallback : DecoderFallback
        {
            public override int MaxCharCount => 1;

            public override DecoderFallbackBuffer CreateFallbackBuffer()
            {
                return new Buffer();
            }

            private class Buffer : DecoderFallbackBuffer
            {
                private int fallbackIndex;
                private string fallbackString;

                public override int Remaining => this.fallbackString.Length - this.fallbackIndex;

                public override bool Fallback(byte[] bytesUnknown, int index)
                {
                    this.fallbackString = Encoding.ASCII.GetString(bytesUnknown.Select(b => (byte)(b & 127)).ToArray());
                    this.fallbackIndex = 0;

                    return true;
                }

                public override char GetNextChar()
                {
                    if (this.Remaining > 0)
                    {
                        return this.fallbackString[this.fallbackIndex++];
                    }
                    else
                    {
                        return '\0';
                    }
                }

                public override bool MovePrevious()
                {
                    if (this.fallbackIndex > 0)
                    {
                        this.fallbackIndex--;
                        return true;
                    }

                    return false;
                }
            }
        }
    }
}