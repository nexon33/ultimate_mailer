using System;

namespace ultimate.mailer
{
    static class Spinner
    {
        private static readonly Random m_random = new Random();

        private static readonly int[] m_partIndices = new int[100];
        private static readonly int[] m_depth = new int[100];
        private static char[] m_symbolsOfTextProcessed;

        public static string Spin(string text)
        {
            m_symbolsOfTextProcessed = new char[text.Length];
            int cur = SpinRecursive(text, 0, text.Length, 0);
            return new string(m_symbolsOfTextProcessed, 0, cur);
        }

        private static int SpinRecursive(String text, int start, int end, int symbolIndex)
        {
            int last = start;

            for (int i = start; i < end; i++)
            {
                if (text[i] == '{')
                {
                    int k = 1;
                    int j = i + 1;
                    int index = 0;

                    m_partIndices[0] = i;
                    m_depth[0] = 1;

                    for (; j < end && k > 0; j++)
                    {
                        if (text[j] == '{') k++;
                        else if (text[j] == '}') k--;
                        else if (text[j] == '|')
                        {
                            if (k == 1)
                            {
                                m_partIndices[++index] = j;
                                m_depth[index] = 1;
                            }
                            else
                            {
                                m_depth[index] = k;
                            }
                        }
                    }
                    if (k == 0)
                    {
                        m_partIndices[++index] = j - 1;

                        int part = m_random.Next(index);

                        text.CopyTo(last, m_symbolsOfTextProcessed, symbolIndex, i - last);
                        symbolIndex += i - last;

                        if (m_depth[part] == 1)
                        {
                            text.CopyTo(m_partIndices[part] + 1, m_symbolsOfTextProcessed, symbolIndex, m_partIndices[part + 1] - m_partIndices[part] - 1);
                            symbolIndex += m_partIndices[part + 1] - m_partIndices[part] - 1;
                        }
                        else
                        {
                            symbolIndex = SpinRecursive(text, m_partIndices[part] + 1, m_partIndices[part + 1], symbolIndex);
                        }

                        i = j - 1;
                        last = j;
                    }
                }
            }

            text.CopyTo(last, m_symbolsOfTextProcessed, symbolIndex, end - last);
            return symbolIndex + end - last;
        }
    }
}
