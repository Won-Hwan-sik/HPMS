using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HPMS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnTasol_Click(object sender, EventArgs e)
        {
            string json = @"{
              ""LTime"": ""2023 10 10 10:33:59"",
              ""PjtCode"": ""DEV1"",
              ""SplrCode"": ""12345679111"",
              ""PourDate"": ""2023 10 10"",
              ""PourNo"": 1,
              ""SchdPourQty"": 1500
            }";

            string encryptedJson = AES_Encrypt(json, yourBase64Key, yourBase64IV);

        }

    }
}
