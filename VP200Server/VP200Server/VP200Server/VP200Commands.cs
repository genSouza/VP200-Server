using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VP200Server
{
    public class VP200Commands
    {
        // desenvolvido por genilton souza

        //alguns comandos básicos do VP200, baseado no manual do usuario
        //basicamente é uma lista de bytes que deve ser enviada para o vp200 via socket, 
        // você pode usar a notação que desejar, por exemplo: 
        // "<ESC> 0x25" no lugar de "27 37", estou utilizando a segunda forma por opção pessoal
        public static byte[] clearScreen() //<ESC> 0x25
        {
            List<byte> data = new List<byte>() {27,37};
            return data.ToArray();
        }

        public static byte[] normalFontSize() //<ESC> 0x42 0x30
        {
            List<byte> data = new List<byte>() { 27, 66, 48 };
            return data.ToArray();
        }

        public static byte[] largeFontSize() //<ESC> 0x42 0x31 
        {
            List<byte> data = new List<byte>() { 27, 66, 49 };
            return data.ToArray();
        }

        public static byte[] alignRightBottom() //<ESC> 0x2e 0x38
        {
            List<byte> data = new List<byte>() { 27, 46, 56 };

            return data.ToArray();

        }

        public static byte[] alignCenterTop() // \x1b\x2e\x31
        {
            List<byte> data = new List<byte>() { 27, 46, 49 };
            return data.ToArray();
        }

        public static byte[] alignCenterBottom() // \x1b\x2e\x37
        {
            List<byte> data = new List<byte>() { 27, 46, 55};
            return data.ToArray();
        }

        public static byte[] alignCenter() // \x1b\x2e\x34
        {
            List<byte> data = new List<byte>() { 27, 46, 52 };
            return data.ToArray();
        }

        public static byte[] enter() //0x0d
        {
            List<byte> data = new List<byte>() { 13 }; //sim, é apenas a "tecla enter"
            return data.ToArray();
        }


        public static byte[] text(string text){ //retornar os bytes de uma string com encoding ascii
            List<byte> data = new List<byte>();
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(text);
            for (int i = 0; i < msg.Length; i++)
            {
                data.Add(msg[i]);
            }
            data.Add(03); //0x03 necessario ser enviado no fim indicando que a lista de bytes da string acabou
            return data.ToArray();
        }
    }
}
