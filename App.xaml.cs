using System;
using System.Threading;
using System.Windows;

namespace Wpf5Laba
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    /// 

    public partial class App : Application
    {
        /// <summary>
        /// максимальное количество допустимых копий
        /// </summary>
        static int maxClientCount = 1;

        /// <summary>
        /// Семафор, который считает
        /// </summary>
        Semaphore sem;
        public App()
        {
            bool semaphoreWasCreated; 
            sem = new Semaphore(maxClientCount - 1, maxClientCount, "SemaphoreExample", out semaphoreWasCreated);
            if (semaphoreWasCreated) 
            {
                Console.WriteLine(@"Полезная работа, первый клиент. Нажми ENTER");
            }
            else if (sem.WaitOne(0))
            {
                Console.WriteLine(@"Полезная работа, второй и последующие клиенты. Нажми ENTER");
            }
            else
            {
                throw new OverflowException("Слишком много было запущено копий");
            }
        }
    }
}
