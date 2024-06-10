using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ComponenteRegistro; // Espacio de nombres que contiene la clase Registro y ListaRegistro.
using Newtonsoft.Json; // Biblioteca para serialización y deserialización JSON.
using System.IO; // Para operaciones de archivo.

namespace Server {
    public class ProgramServer {
        static TcpListener tcpListener; // Objeto TcpListener para escuchar conexiones TCP.
        static Thread listenerThread; // Hilo para escuchar clientes de forma asíncrona.
        static int puerto = 1234; // Puerto en el que escucha el servidor.

        static void Main(string[] args) {
            tcpListener = new TcpListener(IPAddress.Any, puerto); // Inicializa TcpListener en cualquier IP en el puerto especificado.
            listenerThread = new Thread(new ThreadStart(escucharClientes)); // Inicializa el hilo para escuchar clientes.
            listenerThread.Start(); // Inicia el hilo.
        }

        private static void escucharClientes() {
            tcpListener.Start(); // Inicia la escucha de clientes.

            while (true) {
                // Esperar la conexión del cliente
                TcpClient client = tcpListener.AcceptTcpClient(); // Espera a que un cliente se conecte y devuelve un TcpClient para esa conexión.

                // Crear un hilo para manejar la comunicación con el cliente
                Thread clientThread = new Thread(new ParameterizedThreadStart(manejarCliente));
                clientThread.Start(client); // Inicia el hilo para manejar la comunicación con el cliente.
                
                Console.WriteLine("Servidor Escuchando.......");

                // Enviar mensaje al cliente
                EnviarTexto(client, "Bienvenido a nuestro Programa, MANEJA TU PESO CO!!");

                // Imprimir en la consola del servidor
                Console.WriteLine("Cliente conectado");
            }
        }

        private static void manejarCliente(object ObjCliente) {
            TcpClient tcpClient = (TcpClient)ObjCliente; // Convierte el objeto de cliente a TcpClient.
            NetworkStream clientStream = tcpClient.GetStream(); // Obtiene el flujo de red para esta conexión.

            byte[] mensaje = new byte[4096]; // Buffer para almacenar los datos recibidos.
            int bytesRead; // Número de bytes leídos.

            while (true) {
                bytesRead = 0;
                try {
                    bytesRead = clientStream.Read(mensaje, 0, 4096); // Lee datos del flujo de red.
                } catch {
                    break;
                }

                if (bytesRead == 0) break;

                string DataRecibida = Encoding.UTF8.GetString(mensaje, 0, bytesRead); // Convierte los bytes recibidos a una cadena UTF-8.

                string[] partes = DataRecibida.Split('|'); // Divide la cadena recibida en partes separadas por '|'.

                string tipoMsj = partes[0]; // Tipo de mensaje recibido.
                string contenidoMsj = partes[1]; // Contenido del mensaje recibido.
                string m = null; // Mensaje a enviar de vuelta al cliente.

                // Manejar el mensaje según su tipo
                switch (tipoMsj) {
                    case "Mensaje":
                        string mn = JsonConvert.DeserializeObject<string>(contenidoMsj); // Deserializa el contenido del mensaje como una cadena.
                        Console.WriteLine(mn); // Imprime el mensaje en la consola del servidor.
                        break;

                    case "NuevoRegistro":
                        RegistroServer reg = JsonConvert.DeserializeObject<RegistroServer>(contenidoMsj); // Deserializa el contenido del mensaje como un objeto RegistroServer.
                        reg.Imc = reg.calcularIMC(); // Calcula el IMC del registro.
                        m = $"el IMC es igual a {reg.Imc}"; // Construye el mensaje de respuesta con el IMC calculado.

                        // Determina la categoría del IMC y agrega información adicional al mensaje.
                        if (reg.Imc < 18.5) {
                            m += "\nSu peso es inferior al Normal!!";
                        } else if (reg.Imc >= 18.5 && reg.Imc < 25) {
                            m += "\nSu peso es Normal!!";
                        } else if (reg.Imc >= 25 && reg.Imc < 30) {
                            m += "\nSu peso es superior al Normal!!";
                        } else {
                            m += "\nUsted padece Obesidad!!";
                        }

                        try {
                            GuardarRegistroEnArchivo(reg); // Guarda el registro en un archivo JSON.
                            EnviarTexto(tcpClient, m); // Envía el mensaje de vuelta al cliente.
                            Console.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy")} | Registro Ingresado"); // Registra la acción en la consola del servidor.
                        } catch (Exception e) {
                            Console.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy")} | Registro No Ingresado"); // Registra la acción en la consola del servidor si ocurre un error.
                        }
                        break;

                    case "ListaRegistro":
                        string[] men = JsonConvert.DeserializeObject<string[]>(contenidoMsj); // Deserializa el contenido del mensaje como un arreglo de cadenas.

                        if (men.Length == 2) {
                            Console.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy")} | Solicitando Registros"); // Registra la acción en la consola del servidor.

                            // Abre el archivo JSON y obtiene la lista de registros.
                            ListaRegistro l = abrirArchivoJson();

                            // Busca registros por nombre y apellido en la lista de registros.
                            m = l.BuscarRegistrosPorNombreApellido(men[0], men[1]);

                            try {
                                EnviarTexto(tcpClient, m); // Envía los registros encontrados de vuelta al cliente.
                                Console.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy")} | Registros Enviados"); // Registra la acción en la consola del servidor.
                            } catch (Exception e) {
                                Console.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy")} | Registros no enviados"); // Registra la acción en la consola del servidor si ocurre un error.
                            }
                        }
                        break;

                    default:
                        Console.WriteLine($"Tipo de mensaje no reconocido: {tipoMsj}"); // Registra un mensaje de error si el tipo de mensaje no es reconocido.
                        break;
                }
            }
        }

        private static void EnviarMensaje(TcpClient client, string objectType, string message) {
            NetworkStream clientStream = client.GetStream(); // Obtiene el flujo de red para esta conexión.
            byte[] typeBytes = Encoding.UTF8.GetBytes(objectType + "|"); // Convierte el tipo de objeto a bytes UTF-8.
            byte[] buffer = Encoding.UTF8.GetBytes(message); // Convierte el mensaje a bytes UTF-8.

            // Combina los bytes del tipo de objeto y el contenido del objeto.
            byte[] combined = new byte[typeBytes.Length + buffer.Length];
            Buffer.BlockCopy(typeBytes, 0, combined, 0, typeBytes.Length);
            Buffer.BlockCopy(buffer, 0, combined, typeBytes.Length, buffer.Length);

            // Envía los bytes combinados al cliente.
            clientStream.Write(combined, 0, combined.Length);
            clientStream.Flush();
        }

        private static void EnviarTexto(TcpClient tcpClient, string msj) {
            // Serializa el objeto a JSON.
            string json = JsonConvert.SerializeObject(msj);

            // Envía el tipo de objeto y el JSON al servidor.
            EnviarMensaje(tcpClient, "Mensaje", json);
        }

        static void GuardarRegistroEnArchivo(Registro registro) {
            string archivo = "registros.json"; // Nombre del archivo JSON.
            ListaRegistro registrosExistentes = abrirArchivoJson(); // Abre el archivo JSON y obtiene la lista de registros.

            // Agrega el nuevo registro a la lista existente.
            registrosExistentes.AgregarRegistro(registro);

            // Convierte la lista completa a JSON.
            var json = JsonConvert.SerializeObject(registrosExistentes, Formatting.Indented);

            // Escribe el JSON en el archivo.
            File.WriteAllText(archivo, json);
        }

        private static ListaRegistro abrirArchivoJson() {
            string archivo = "registros.json"; // Nombre del archivo JSON.
            ListaRegistro registrosExistentes = null; // Inicializa la lista de registros como nula.

            if (File.Exists(archivo)) {
                // Lee los registros existentes desde el archivo JSON.
                var jsonExistente = File.ReadAllText(archivo);
                registrosExistentes = JsonConvert.DeserializeObject<ListaRegistro>(jsonExistente) ?? new ListaRegistro();
            }

            return registrosExistentes; // Devuelve la lista de registros existentes.
        }
    }
}
