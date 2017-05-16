using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace Service_Importar_Data
{
    [RunInstaller(true)]
    public partial class Install_Import : System.Configuration.Install.Installer
    {
        public Install_Import()
        {
            InitializeComponent();
        }
    }
}
