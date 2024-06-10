using System.Collections.Generic; // Necesario para el uso de listas.

namespace ComponenteRegistro {
    // Clase que representa una lista de registros.
    public class ListaRegistro {
        private List<Registro> registros; // Lista privada que contiene los registros.

        // Propiedad para acceder a la lista de registros.
        public List<Registro> Registros { get => registros; set => registros = value; }

        // Constructor que inicializa la lista de registros.
        public ListaRegistro() {
            registros = new List<Registro>();
        }

        // Método para agregar un nuevo registro a la lista.
        public void AgregarRegistro(Registro registro) {
            registros.Add(registro);
        }

        // Método para obtener todos los registros.
        public List<Registro> ObtenerRegistros() {
            return registros;
        }

        // Método para buscar registros por nombre y apellido.
        public string BuscarRegistrosPorNombreApellido(string nombre, string apellido) {
            // Encuentra todos los registros que coinciden con el nombre y apellido proporcionados.
            var resultados = registros.FindAll(r => r.Nombre == nombre && r.Apellido == apellido);
            if (resultados.Count == 0) {
                return "No se encontraron registros con el nombre y apellido especificados.";
            }

            // Encabezado de la tabla de composición corporal y rangos de IMC.
            string salida = @"
                            Composición corporal                  Índice de masa corporal (IMC)
                            Peso inferior al normal               Menos de 18.5
                            Normal                                18.5 - 24.9
                            Peso superior al normal               25.0 - 29.9
                            Obesidad                              Más de 30.0
                            ";

            // Encabezado de los registros encontrados.
            salida += $"\nRegistros de: {nombre} {apellido}\n";
            salida += "-------------------------------------------------------------\n";
            salida += string.Format("{0,5} {1,-15} {2,-10} {3,-10} {4,-10}\n", "#", "Peso (kg)", "Altura (m)", "IMC", "Fecha");
            salida += "-------------------------------------------------------------\n";

            int contador = 1;
            // Itera a través de los registros encontrados.
            foreach (var registro in resultados) {
                // Convierte el peso a kilogramos si es necesario.
                double pesoKg = registro.Peso;
                switch (registro.MedidaPeso.ToUpper()) {
                    case "LB":
                        pesoKg = registro.Peso * 0.453592; // 1 libra = 0.453592 kg
                        break;
                    case "G":
                        pesoKg = registro.Peso / 1000; // 1 gramo = 0.001 kg
                        break;
                    case "KG":
                        pesoKg = registro.Peso;
                        break;
                    case "ONZA":
                        pesoKg = registro.Peso * 0.0283495; // 1 onza = 0.0283495 kg
                        break;
                }

                // Convierte la altura a metros si es necesario.
                double alturaMetros = registro.Altura;
                switch (registro.MedidaAltura.ToUpper()) {
                    case "CM":
                        alturaMetros = registro.Altura / 100; // 1 cm = 0.01 m
                        break;
                    case "PIES":
                        alturaMetros = registro.Altura * 0.3048; // 1 pie = 0.3048 m
                        break;
                    case "M":
                        alturaMetros = registro.Altura;
                        break;
                    case "PULGADAS":
                        alturaMetros = registro.Altura * 0.0254; // 1 pulgada = 0.0254 m
                        break;
                }

                // Formatea la salida con las unidades convertidas.
                salida += string.Format("{0,5} {1,-15:F2} {2,-10:F2} {3,-10:F2} {4,-10}\n",
                                        contador,
                                        pesoKg,
                                        alturaMetros,
                                        registro.Imc,
                                        registro.Fecha.ToString("dd/MM/yyyy"));
                salida += "-------------------------------------------------------------\n";
                contador++;
            }
            return salida; // Devuelve la cadena de resultados formateada.
        }
    }
}
