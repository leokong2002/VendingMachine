using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVSSoftware
{
    
    class lang
    {
        private int lang_code;
        //Set Language
        public int Set_lang
        {
            get
            {
                return this.lang_code;
            }
            set
            {
                lang_code = value;
            }
        }
        

        //Greeting
        public string Greeting()
        {
            if(lang_code == 1)
            {
                return "Bienvenido, por favor ingrese su tarjeta";
            }
            return "Welcome, Please Enter Your Card";
        }
        
        //Allergen
        public string Allergen(int agn)
        {
            if(lang_code == 1)
            {
                switch (agn)
                {
                    case 0: return "¿Usted es Alérgico a Nada?";
                    case 1: return "¿Usted es Alérgico al Rojo?"; 
                    case 10: return "¿Usted es Alérgico al Verde?";
                    case 100: return "¿Eres Alérgico al Azul?";
                    case 1000: return "¿Eres Alérgico al Amarillo?";
                    case 11: return "¿Usted es Alérgico a la Naranja?";
                    case 101: return "¿Usted es Alérgico al Blanco?";
                    case 1001: return "¿Usted es Alérgico al Negro?";
                }
            }

            switch (agn)
            {
                case 0: return "You are Allergic to Nothing?";
                case 1: return "You are Allergic to Red?";
                case 10: return "You are Allergic to Green?";
                case 100: return "You are Allergic to Blue?";
                case 1000: return "You are Allergic to Yellow?";
                case 11: return "You are Allergic to Orange?";
                case 101: return "You are Allergic to White?";
                case 1001: return "You are Allergic to Black?";
            }
            return "ERROR";
        }

        //Dispence
        public string Disp(int data)
        {
            if(data == 0)
            {
                if(lang_code == 1)
                {
                    return "Dispensación de bocadillos. Disfrutar";
                }
                return "Dispensing Snacks. Enjoy";
            }
            if(lang_code == 1)
            {
                return "Lo sentimos, no tenemos suficiente";
            }
            return "Sorry, We Do Not Have Enough";
        }

        //Error
        public string Error()
        {
            if(lang_code == 1)
            {
                return "Error. Por favor contacte a un operador";
            }
            return "Error. Please Contact an Operator";
        }

        public string Sel_disp()
        {
            if(lang_code == 1)
            {
                return "Seleccione el número de bocadillos";
            }
            return "Please Select Number of Snacks";
        }
    }

}
