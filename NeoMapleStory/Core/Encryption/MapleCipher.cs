﻿using System;

namespace NeoMapleStory.Core.Encryption
{
    public sealed class MapleCipher
    {
        public enum CipherType : byte
        {
            Encrypt,
            Decrypt
        }

        public const int IvLength = 4;

        public static readonly byte[] SKey =
        {
            0x13, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0xB4, 0x00, 0x00, 0x00,
            0x1B, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x33, 0x00, 0x00, 0x00, 0x52, 0x00, 0x00, 0x00
        };

        private static readonly byte[] SShiftKey =
        {
            0xEC, 0x3F, 0x77, 0xA4, 0x45, 0xD0, 0x71, 0xBF, 0xB7, 0x98, 0x20, 0xFC, 0x4B, 0xE9, 0xB3, 0xE1,
            0x5C, 0x22, 0xF7, 0x0C, 0x44, 0x1B, 0x81, 0xBD, 0x63, 0x8D, 0xD4, 0xC3, 0xF2, 0x10, 0x19, 0xE0,
            0xFB, 0xA1, 0x6E, 0x66, 0xEA, 0xAE, 0xD6, 0xCE, 0x06, 0x18, 0x4E, 0xEB, 0x78, 0x95, 0xDB, 0xBA,
            0xB6, 0x42, 0x7A, 0x2A, 0x83, 0x0B, 0x54, 0x67, 0x6D, 0xE8, 0x65, 0xE7, 0x2F, 0x07, 0xF3, 0xAA,
            0x27, 0x7B, 0x85, 0xB0, 0x26, 0xFD, 0x8B, 0xA9, 0xFA, 0xBE, 0xA8, 0xD7, 0xCB, 0xCC, 0x92, 0xDA,
            0xF9, 0x93, 0x60, 0x2D, 0xDD, 0xD2, 0xA2, 0x9B, 0x39, 0x5F, 0x82, 0x21, 0x4C, 0x69, 0xF8, 0x31,
            0x87, 0xEE, 0x8E, 0xAD, 0x8C, 0x6A, 0xBC, 0xB5, 0x6B, 0x59, 0x13, 0xF1, 0x04, 0x00, 0xF6, 0x5A,
            0x35, 0x79, 0x48, 0x8F, 0x15, 0xCD, 0x97, 0x57, 0x12, 0x3E, 0x37, 0xFF, 0x9D, 0x4F, 0x51, 0xF5,
            0xA3, 0x70, 0xBB, 0x14, 0x75, 0xC2, 0xB8, 0x72, 0xC0, 0xED, 0x7D, 0x68, 0xC9, 0x2E, 0x0D, 0x62,
            0x46, 0x17, 0x11, 0x4D, 0x6C, 0xC4, 0x7E, 0x53, 0xC1, 0x25, 0xC7, 0x9A, 0x1C, 0x88, 0x58, 0x2C,
            0x89, 0xDC, 0x02, 0x64, 0x40, 0x01, 0x5D, 0x38, 0xA5, 0xE2, 0xAF, 0x55, 0xD5, 0xEF, 0x1A, 0x7C,
            0xA7, 0x5B, 0xA6, 0x6F, 0x86, 0x9F, 0x73, 0xE6, 0x0A, 0xDE, 0x2B, 0x99, 0x4A, 0x47, 0x9C, 0xDF,
            0x09, 0x76, 0x9E, 0x30, 0x0E, 0xE4, 0xB2, 0x94, 0xA0, 0x3B, 0x34, 0x1D, 0x28, 0x0F, 0x36, 0xE3,
            0x23, 0xB4, 0x03, 0xD8, 0x90, 0xC8, 0x3C, 0xFE, 0x5E, 0x32, 0x24, 0x50, 0x1F, 0x3A, 0x43, 0x8A,
            0x96, 0x41, 0x74, 0xAC, 0x52, 0x33, 0xF0, 0xD9, 0x29, 0x80, 0xB1, 0x16, 0xD3, 0xAB, 0x91, 0xB9,
            0x84, 0x7F, 0x61, 0x1E, 0xCF, 0xC5, 0xD1, 0x56, 0x3D, 0xCA, 0xF4, 0x05, 0xC6, 0xE5, 0x08, 0x49
        };

        private readonly AesCipher m_mAesCipher;

        private readonly short m_mMajorVersion;
        private readonly Action<byte[]> m_mTransformer;
        public readonly byte[] MIv;

        public MapleCipher(short majorVersion, byte[] iv, CipherType transformDirection)
        {
            m_mMajorVersion = (short) (((majorVersion >> 8) & 0xFF) | ((majorVersion << 8) & 0xFF00));

            MIv = new byte[IvLength];
            Buffer.BlockCopy(iv, 0, MIv, 0, IvLength);

            m_mAesCipher = new AesCipher(SKey);

            m_mTransformer =
                transformDirection == CipherType.Encrypt ? EncryptTransform : new Action<byte[]>(DecryptTransform);
        }

        public void Transform(byte[] data)
        {
            m_mTransformer(data);

            byte[] newIv = {0xF2, 0x53, 0x50, 0xC6};

            for (var i = 0; i < IvLength; i++)
            {
                var input = MIv[i];
                var tableInput = SShiftKey[input];

                newIv[0] += (byte) (SShiftKey[newIv[1]] - input);
                newIv[1] -= (byte) (newIv[2] ^ tableInput);
                newIv[2] ^= (byte) (SShiftKey[newIv[3]] + input);
                newIv[3] -= (byte) (newIv[0] - tableInput);

                var val = BitConverter.ToUInt32(newIv, 0);
                var val2 = val >> 0x1D;
                val <<= 0x03;
                val2 |= val;
                newIv[0] = (byte) (val2 & 0xFF);
                newIv[1] = (byte) ((val2 >> 8) & 0xFF);
                newIv[2] = (byte) ((val2 >> 16) & 0xFF);
                newIv[3] = (byte) ((val2 >> 24) & 0xFF);
            }

            Buffer.BlockCopy(newIv, 0, MIv, 0, IvLength);
        }

        private void EncryptTransform(byte[] data)
        {
            var size = data.Length;

            int j;
            byte a, c;
            for (var i = 0; i < 3; i++)
            {
                a = 0;
                for (j = size; j > 0; j--)
                {
                    c = data[size - j];
                    c = RollLeft(c, 3);
                    c = (byte) (c + j);
                    c ^= a;
                    a = c;
                    c = RollRight(a, j);
                    c ^= 0xFF;
                    c += 0x48;
                    data[size - j] = c;
                }
                a = 0;
                for (j = data.Length; j > 0; j--)
                {
                    c = data[j - 1];
                    c = RollLeft(c, 4);
                    c = (byte) (c + j);
                    c ^= a;
                    a = c;
                    c ^= 0x13;
                    c = RollRight(c, 3);
                    data[j - 1] = c;
                }
            }

            m_mAesCipher.Transform(data, MIv);
        }

        private void DecryptTransform(byte[] data)
        {
            var size = data.Length;

            m_mAesCipher.Transform(data, MIv);

            int j;
            byte a, b, c;
            for (var i = 0; i < 3; i++)
            {
                a = 0;
                b = 0;
                for (j = size; j > 0; j--)
                {
                    c = data[j - 1];
                    c = RollLeft(c, 3);
                    c ^= 0x13;
                    a = c;
                    c ^= b;
                    c = (byte) (c - j);
                    c = RollRight(c, 4);
                    b = a;
                    data[j - 1] = c;
                }
                a = 0;
                b = 0;
                for (j = size; j > 0; j--)
                {
                    c = data[size - j];
                    c -= 0x48;
                    c ^= 0xFF;
                    c = RollLeft(c, j);
                    a = c;
                    c ^= b;
                    c = (byte) (c - j);
                    c = RollRight(c, 3);
                    b = a;
                    data[size - j] = c;
                }
            }
        }

        public byte[] GetPacketHeader(int length)
        {
            var iiv = MIv[3] & 0xFF;
            iiv |= (MIv[2] << 8) & 0xFF00;

            iiv ^= m_mMajorVersion;
            var mlength = ((length << 8) & 0xFF00) | RightMove(length, 8);
            var xoredIv = iiv ^ mlength;

            var ret = new byte[4];
            ret[0] = (byte) (RightMove(iiv, 8) & 0xFF);
            ret[1] = (byte) (iiv & 0xFF);
            ret[2] = (byte) (RightMove(xoredIv, 8) & 0xFF);
            ret[3] = (byte) (xoredIv & 0xFF);
            return ret;
        }

        public static int GetPacketLength(byte[] packetHeader)
        {
            return (packetHeader[0] + (packetHeader[1] << 8)) ^ (packetHeader[2] + (packetHeader[3] << 8));
        }

        public bool CheckPacket(byte[] packet)
        {
            return (((packet[0] ^ MIv[2]) & 0xFF) == ((m_mMajorVersion >> 8) & 0xFF)) &&
                   (((packet[1] ^ MIv[3]) & 0xFF) == (m_mMajorVersion & 0xFF));
        }

        private static byte RollLeft(byte value, int shift)
        {
            var num = (uint) (value << (shift%8));
            return (byte) ((num & 0xff) | (num >> 8));
        }

        private static byte RollRight(byte value, int shift)
        {
            var num = (uint) ((value << 8) >> (shift%8));
            return (byte) ((num & 0xff) | (num >> 8));
        }

        private static int RightMove(int value, int pos)
        {
            if (pos != 0) //移动 0 位时直接返回原值
            {
                var mask = 0x7fffffff;
                value >>= 1;
                value &= mask;
                value >>= pos - 1;
            }
            return value;
        }
    }
}