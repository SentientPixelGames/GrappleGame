using System;

namespace GrappleGame
{
#if WINDOWS || XBOX
    static class Program
    {
        static void Main(string[] args)
        {
            using (mainScript game = new mainScript())
            {
                game.Run();
            }
        }
    }
#endif
}

