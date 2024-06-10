using System;

namespace ComponenteRegistro {
    // La clase Registro representa un registro individual con información personal y medidas.
    public class Registro {
        // Campos privados para almacenar la información del registro.
        private string nombre;           // Almacena el nombre del registro.
        private string apellido;         // Almacena el apellido del registro.
        private DateTime fecha;          // Almacena la fecha de registro.
        private double peso;             // Almacena el peso del registro.
        private string medidaPeso;       // Almacena la medida del peso (Ej. KG, LB).
        private double altura;           // Almacena la altura del registro.
        private string medidaAltura;     // Almacena la medida de la altura (Ej. CM, PIES).
        private double imc;              // Almacena el índice de masa corporal (IMC) del registro.

        // Constructor sin parámetros que inicializa un registro vacío.
        public Registro() { }

        // Constructor que inicializa un registro con todos los valores, incluido el IMC.
        public Registro(string nombre, string apellido, DateTime fecha, double peso, string medidaPeso, double altura, string medidaAltura, double imc) {
            this.nombre = nombre;
            this.apellido = apellido;
            this.fecha = fecha;
            this.peso = peso;
            this.medidaPeso = medidaPeso;
            this.altura = altura;
            this.medidaAltura = medidaAltura;
            this.imc = imc;
        }

        // Constructor que inicializa un registro sin el valor del IMC.
        public Registro(string nombre, string apellido, DateTime fecha, double peso, string medidaPeso, double altura, string medidaAltura) {
            this.nombre = nombre;
            this.apellido = apellido;
            this.fecha = fecha;
            this.peso = peso;
            this.medidaPeso = medidaPeso;
            this.altura = altura;
            this.medidaAltura = medidaAltura;
            this.imc = 0.0; // Se establece el IMC en 0.0 si no se proporciona.
        }

        // Propiedades públicas para acceder y modificar los campos privados.
        public string Nombre { get => nombre; set => nombre = value; }                     // Propiedad para el nombre.
        public string Apellido { get => apellido; set => apellido = value; }               // Propiedad para el apellido.
        public DateTime Fecha { get => fecha; set => fecha = value; }                      // Propiedad para la fecha.
        public double Peso { get => peso; set => peso = value; }                           // Propiedad para el peso.
        public string MedidaPeso { get => medidaPeso; set => medidaPeso = value; }         // Propiedad para la medida del peso.
        public double Altura { get => altura; set => altura = value; }                     // Propiedad para la altura.
        public string MedidaAltura { get => medidaAltura; set => medidaAltura = value; }   // Propiedad para la medida de la altura.
        public double Imc { get => imc; set => imc = value; }                              // Propiedad para el IMC.
    }
}
