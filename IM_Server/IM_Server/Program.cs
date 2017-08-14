using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM_Server
{
    /// <summary>
    /// Entry point for the program.
    /// Creats the controller and provides static access to it (for client communications).
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The controller.
        /// </summary>
        private static Controller controller;

        /// <summary>
        /// Static reference to the created Controller object.
        /// This is necessary (static especially) because the ClientCommunication which is used by WebSocketSharp
        /// does not and cannot have any parameters passed to it. It therefore has no access to the Controller, or
        /// any runtime objects. This method allows the ClientCommunication to gain access to the Controller (which
        /// is extremely vital in this program) by calling Program.GetController().
        /// </summary>
        /// <returns>An interface for the Controller providing the only method the ClientCommunication will need access to.</returns>
        public static ControlInterface GetController()
        {
            return controller;
        }

        /// <summary>
        /// Entry point for the program.
        /// </summary>
        /// <param name="args">Unused.</param>
        public static void Main(string[] args)
        {
            controller = new Controller();
//            controller.Run();
            //Run controller on new thread to check test cases
            System.Threading.Thread newThread = new System.Threading.Thread(
                new System.Threading.ThreadStart(controller.Run));
            newThread.Start();
            TestCases.RunTestCases();
        }
    }
}
