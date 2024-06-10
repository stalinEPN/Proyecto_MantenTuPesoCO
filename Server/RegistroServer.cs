using System;
using ComponenteRegistro; // Espacio de nombres que contiene la clase Registro.

namespace Server {
    // La clase RegistroServer hereda de Registro y añade funcionalidades específicas del servidor.
    public class RegistroServer : Registro {
        // Constructor que inicializa un RegistroServer y calcula el IMC.
        public RegistroServer(string nombre, string apellido, DateTime fecha, double peso, string medidaPeso, double altura, string medidaAltura)
            : base(nombre, apellido, fecha, peso, medidaPeso, altura, medidaAltura) {
            // Calcula y establece el IMC utilizando el método calcularIMC.
            base.Imc = calcularIMC();
        }

        // Método para calcular el IMC basado en el peso y la altura.
        public double calcularIMC() {
            double peso = Peso; // Obtiene el peso del registro.
            double altura = Altura; // Obtiene la altura del registro.

            // Si el peso está en libras, se convierte la altura a pulgadas y se calcula el IMC.
            if (MedidaPeso.Equals("LB")) {
                switch (MedidaAltura) {
                    case "CM":
                        altura *= 0.39337; // Convierte centímetros a pulgadas.
                        break;
                    case "M":
                        altura *= 39.37; // Convierte metros a pulgadas.
                        break;
                    case "PIES":
                        altura *= 12; // Convierte pies a pulgadas.
                        break;
                    case "PULGADAS":
                        break; // No es necesario convertir si ya está en pulgadas.
                    default:
                        throw new ArgumentException("Unidad de medida de altura no válida", nameof(MedidaAltura));
                }
                // Calcula el IMC utilizando la fórmula para libras y pulgadas.
                return (peso / Math.Pow(altura, 2)) * 703;
            } else {
                // Si el peso no está en libras, se convierte la altura a metros si es necesario.
                switch (MedidaAltura) {
                    case "CM":
                        altura /= 100; // Convierte centímetros a metros.
                        break;
                    case "M":
                        break; // No es necesario convertir si ya está en metros.
                    case "PIES":
                        altura *= 0.3048; // Convierte pies a metros.
                        break;
                    case "PULGADAS":
                        altura *= 0.0254; // Convierte pulgadas a metros.
                        break;
                    default:
                        throw new ArgumentException("Unidad de medida de altura no válida", nameof(MedidaAltura));
                }

                // Convierte el peso a kilogramos si es necesario.
                switch (MedidaPeso) {
                    case "G":
                        peso /= 1000; // Convierte gramos a kilogramos.
                        break;
                    case "KG":
                        break; // No es necesario convertir si ya está en kilogramos.
                    case "LB":
                        peso *= 0.453592; // Convierte libras a kilogramos.
                        break;
                    case "ONZA":
                        peso *= 0.0283495; // Convierte onzas a kilogramos.
                        break;
                    default:
                        throw new ArgumentException("Unidad de medida de peso no válida", nameof(peso));
                }
            }

            // Calcula y devuelve el IMC utilizando la fórmula para kilogramos y metros.
            return (peso / Math.Pow(altura, 2));
        }
    }
}
