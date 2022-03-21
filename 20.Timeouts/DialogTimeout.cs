using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace _20.Timeouts
{
    class DialogTimeout
    {
        private static CancellationTokenSource cancellationTokenSource;
        private static bool dialogClosed;

        /// <summary>
        /// We show a dialog and close it after 15 second. But if the user close it, we do not need to do anything
        /// </summary>
        public static void Demo()
        {
            cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            var tDialog = new Thread(() => { OpenDialog(cancellationTokenSource.Token); });
            tDialog.Start();

            var tAction = new Thread(() => WaitForUserAction());
            tAction.Start();

             
        }


        static void WaitForUserAction()
        {
            while (!dialogClosed)
            {
                var key = Console.ReadKey();
                if (key.KeyChar == 'c' && !dialogClosed)
                {
                    Console.WriteLine("The user wants to close the dialog");
                    cancellationTokenSource.Cancel();
                    return;
                }
            }
        }

        static void OpenDialog(CancellationToken cancellationToken)
        {
            Console.WriteLine("Dialog is open");
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Dialog closed");
                    dialogClosed = true;
                    return;
                }
            }
        }

    }
}
