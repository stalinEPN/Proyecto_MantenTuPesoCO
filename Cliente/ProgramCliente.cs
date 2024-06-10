using System;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using ComponenteRegistro; // Espacio de nombres que contiene la clase Registro.

namespace Cliente {
    internal class ProgramCliente {
        static void Main(string[] args) {
            try {
                string ip = "localhost";
                TcpClient tcpClient = new TcpClient(ip, 1234); // Conexión al servidor TCP en localhost y puerto 1234.
                NetworkStream clientStream = tcpClient.GetStream(); // Flujo de red para la comunicación con el servidor.

                // Leer mensaje de bienvenida del servidor
                byte[] mensaje = new byte[4096];
                int bytesRead = clientStream.Read(mensaje, 0, 4096);
                String bienvenida = Encoding.UTF8.GetString(mensaje, 0, bytesRead);
                string[] parts = bienvenida.Split('|');
                string objectContent = parts[1];
                string mensajeRecibido = JsonConvert.DeserializeObject<string>(objectContent);
                Console.WriteLine($"{mensajeRecibido}");

                string[] msj = null;
                int opc = 0;

                // Menú principal del cliente
                while (opc != 3) {
                    menu(); // Mostrar el menú de opciones
                    if (!int.TryParse(Console.ReadLine().ToString(), out opc)) {
                        Console.WriteLine("No contengo esa opcion");
                        continue;
                    }

                    switch (opc) {
                        case 1:
                            Registro reg = ingresarRegistro(); // Función para ingresar un nuevo registro
                            enviarRegistro(tcpClient, reg); // Enviar el registro al servidor
                            parts = recibirMensaje(clientStream); // Recibir mensaje de respuesta del servidor
                            Console.WriteLine(JsonConvert.DeserializeObject<string>(parts[1])); // Imprimir mensaje de respuesta
                            break;
                        case 2:
                            string[] nombres = new string[2];

                            Console.Write("Ingresa tu nombre: ");
                            nombres[0] = Console.ReadLine();
                            Console.Write("Ingresa tu apellido: ");
                            nombres[1] = Console.ReadLine();

                            SolicitarRegistros(tcpClient, nombres); // Solicitar registros al servidor
                            parts = recibirMensaje(clientStream); // Recibir mensaje de respuesta del servidor
                            Console.WriteLine(JsonConvert.DeserializeObject<string>(parts[1])); // Imprimir mensaje de respuesta
                            break;
                        case 3:
                            Console.WriteLine("Gracias Por usar nuestro Programa");
                            break;
                        default:
                            Console.WriteLine("No contengo esa opcion");
                            break;
                    }
                }
            } catch (Exception e) {
                Console.WriteLine($"Error: {e}");
            }
        }

        // Mostrar el menú principal al usuario
        private static void menu() {
            Console.WriteLine("\n --------------------------------------------------\n" +
                                "|                  Menu Principal                  |\n" +
                                " --------------------------------------------------\n");
            Console.Write("1. Calcular IMC\n2. Ver Registros\n3. Salir\ningrese una opcion: ");
        }

        // Función para ingresar un nuevo registro de usuario
        private static Registro ingresarRegistro() {
            int opc = 0;
            Registro nuevoReg = new Registro();

            Console.Write("Ingrese su nombre  : ");
            nuevoReg.Nombre = Console.ReadLine();
            Console.Write("Ingrese su apellido: ");
            nuevoReg.Apellido = Console.ReadLine();

            // Elegir la medida del peso
            while (opc == 0) {
                Console.WriteLine(" ---------------------------\n" +
                                  "| Escoja la medida del peso |\n" +
                                  " ---------------------------");
                Console.Write("1. Libra\n2. Kilo Gramo\n3. Gramo\n4. Onza\nIngrese una opcion: ");
                try {
                    opc = int.Parse(Console.ReadLine());
                    switch (opc) {
                        case 1:
                            nuevoReg.MedidaPeso = "LB";
                            break;
                        case 2:
                            nuevoReg.MedidaPeso = "KG";
                            break;
                        case 3:
                            nuevoReg.MedidaPeso = "G";
                            break;
                        case 4:
                            nuevoReg.MedidaPeso = "ONZA";
                            break;
                        default:
                            opc = 0;
                            break;
                    }
                } catch (FormatException) {
                    Console.WriteLine("NO contengo esa opcion!!!!!\n");
                }
            }

            // Ingresar el peso del usuario
            while (true) {
                try {
                    Console.Write("Ingrese su peso, use la coma(,) como punto decimal en caso de necesitar (ejm: 1,50): ");
                    double P = double.Parse(Console.ReadLine());
                    if (P <= 0)
                        throw new FormatException();

                    nuevoReg.Peso = P;
                    break;

                } catch (FormatException) {
                    Console.WriteLine("Valor incorrecto!!!!!\n");
                }
            }

            opc = 0;

            // Elegir la medida de la altura
            while (opc == 0) {
                Console.WriteLine(" -------------------------------\n" +
                                  "| Escoja la medida de la altura |\n" +
                                  " -------------------------------");
                Console.Write("1. Centimetros\n2. Metros\n3. Pies\n4. Pulgadas\nIngrese una opcion: ");
                try {
                    opc = int.Parse(Console.ReadLine());
                    switch (opc) {
                        case 1:
                            nuevoReg.MedidaAltura = "CM";
                            break;
                        case 2:
                            nuevoReg.MedidaAltura = "M";
                            break;
                        case 3:
                            nuevoReg.MedidaAltura = "PIES";
                            break;
                        case 4:
                            nuevoReg.MedidaAltura = "PULGADAS";
                            break;
                        default:
                            opc = 0;
                            break;
                    }
                } catch (FormatException) {
                    Console.WriteLine("NO contengo esa opcion!!!!!\n");
                }
            }

            // Ingresar la altura del usuario
            while (true) {
                try {
                    Console.Write("Ingrese su altura, use la coma(,) como punto decimal en caso de necesitar (ejm: 1,50): ");
                    double A = double.Parse(Console.ReadLine());
                    if (A <= 0)
                        throw new FormatException();

                    nuevoReg.Altura = A;
                    break;

                } catch (FormatException) {
                    Console.WriteLine("Valor incorrecto!!!!!\n");
                }
            }

            nuevoReg.Fecha = DateTime.Now; // Asignar la fecha actual al registro

            return nuevoReg;
        }

        // Función para recibir un mensaje del servidor
        private static string[] recibirMensaje(NetworkStream clientStream) {
            byte[] mensaje = new byte[4096];
            int bytesRead = clientStream.Read(mensaje, 0, 4096);
            String msj = Encoding.UTF8.GetString(mensaje, 0, bytesRead);
            string[] parts = msj.Split('|');
            if (parts.Length == 2) {
                string objectType = parts[0]; // Tipo de objeto recibido
                string objectContent = parts[1]; // Contenido del objeto
                // Aquí se podría utilizar objectType si se necesita saber qué tipo de objeto estás recibiendo
            }

            return parts; // Devuelve el mensaje recibido
        }

        // Función para enviar un mensaje al servidor
        private static void EnviarMensaje(TcpClient client, string objectType, string message) {
            NetworkStream clientStream = client.GetStream();
            byte[] typeBytes = Encoding.UTF8.GetBytes(objectType + "|");
            byte[] buffer = Encoding.UTF8.GetBytes(message);

            // Combina los bytes del tipo de objeto y el contenido del objeto
            byte[] combined = new byte[typeBytes.Length + buffer.Length];
            Buffer.BlockCopy(typeBytes, 0, combined, 0, typeBytes.Length);
            Buffer.BlockCopy(buffer, 0, combined, typeBytes.Length, buffer.Length);

            // Enviar los bytes combinados al servidor
            clientStream.Write(combined, 0, combined.Length);
            clientStream.Flush(); // Limpiar el flujo de datos
        }

        // Función para enviar un registro al servidor
        private static void enviarRegistro(TcpClient tcpClient, Registro reg) {
            string json = JsonConvert.SerializeObject(reg); // Serializa el objeto a JSON
            EnviarMensaje(tcpClient, "NuevoRegistro", json); // Envia el JSON al servidor con el tipo de objeto "NuevoRegistro"
        }

        // Función para solicitar registros al servidor
        private static void SolicitarRegistros(TcpClient tcpClient, string[] nombres) {
            string json = JsonConvert.SerializeObject(nombres); // Serializa el arreglo de nombres a JSON
            EnviarMensaje(tcpClient, "ListaRegistro", json); // Envia el JSON al servidor con el tipo de objeto "ListaRegistro"
        }
    }
}
