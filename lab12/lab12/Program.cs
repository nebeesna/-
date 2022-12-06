using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCL.Net.Extensions;
using OpenCL.Net;
using System.Diagnostics;

namespace lab12
{
    class TestProgram
    {

        static void matrixTransformation(float[,] matrix, int size)
        {   
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    matrix[i, j] = (float)Math.Round(Math.Sin(matrix[i, j]) + Math.Cos(matrix[i, j]), 2);
                }
            }
        }
        static void Main(string[] args)
        {
            Console.Write("Input matrix size: ");
            int size = Convert.ToInt32(Console.ReadLine());

            // Сreate a random matrix
            float[,] matrix = new float[size, size];
            Random random = new Random();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    matrix[i, j] = random.Next(0, 10);
                }
            }

            Stopwatch clock = new Stopwatch();
            clock.Start();
            matrixTransformation(matrix, size);
            clock.Stop();
            Console.WriteLine($"\nProgram completed in: {clock.Elapsed.TotalMilliseconds} miliseconds");

            clock.Restart();

            Event event0; ErrorCode err;
            Platform[] platforms = Cl.GetPlatformIDs(out err);
            Device[] devices = Cl.GetDeviceIDs(platforms[0], DeviceType.Gpu, out err);
            Device device = devices[0]; //cl_device_id device;
            Context context = Cl.CreateContext(null, 1, devices, null, IntPtr.Zero, out err);
            CommandQueue cmdQueue = Cl.CreateCommandQueue(context, device, CommandQueueProperties.None, out err);



            string programSource = @"
                __kernel void matrixTransformation(__global float** matrix,int n) 
                { 
                    for(int i = 0; i < n;i++)
                    {
                        for(int j = 0; j < n;j++)
                        {
                             matrix[i][j] = sin(matrix[i][j]) + cos(matrix[i][j]);
                        }
                    }      
                };";
            Program program = Cl.CreateProgramWithSource(context, 1, new[] { programSource }, null, out err);
            Cl.BuildProgram(program, 0, null, string.Empty, null, IntPtr.Zero);  


            // Check for any compilation errors
            if (Cl.GetProgramBuildInfo(program, device, ProgramBuildInfo.Status, out err).CastTo<BuildStatus>() != BuildStatus.Success)
            {
                if (err != ErrorCode.Success)
                    Console.WriteLine("ERROR: " + "Cl.GetProgramBuildInfo" + " (" + err.ToString() + ")");
                Console.WriteLine("Cl.GetProgramBuildInfo != Success");
                Console.WriteLine(Cl.GetProgramBuildInfo(program, device, ProgramBuildInfo.Log, out err));
            }

            Kernel kernel = Cl.CreateKernel(program, "matrixTransformation", out err);

            Mem memInputA = (Mem)Cl.CreateBuffer(context, MemFlags.ReadOnly, sizeof(int) * size * size, out err);
            Mem memInputB = (Mem)Cl.CreateBuffer(context, MemFlags.ReadOnly, sizeof(int) * size * size, out err);

            Mem memoutput = (Mem)Cl.CreateBuffer(context, MemFlags.WriteOnly, sizeof(int) * size * size, out err);
            float[] a = new float[size * size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    a[i * size + j] = matrix[i, j];
                }
            }

            Cl.EnqueueWriteBuffer(cmdQueue, (IMem)memInputA, Bool.True, IntPtr.Zero, new IntPtr(sizeof(int) * size * size), a, 0, null, out event0);

            IntPtr[] localWorkSize = new IntPtr[2], globalWorkSize = new IntPtr[2];
            globalWorkSize[0] = new IntPtr(size);
            globalWorkSize[1] = new IntPtr(size);
            localWorkSize[0] = new IntPtr(sizeof(int));
            localWorkSize[1] = new IntPtr(sizeof(int));

            Cl.SetKernelArg(kernel, 0, new IntPtr(4), memInputA);
            Cl.SetKernelArg(kernel, 1, new IntPtr(4), memInputB);
            Cl.SetKernelArg(kernel, 2, new IntPtr(4), memoutput);
            Cl.SetKernelArg(kernel, 3, new IntPtr(4), size);
            IntPtr[] workGroupSizePtr = new IntPtr[] { new IntPtr(size) };
            Cl.EnqueueNDRangeKernel(cmdQueue, kernel, 2, null, workGroupSizePtr, localWorkSize, 0, null, out event0);


            Cl.Finish(cmdQueue);

            Cl.EnqueueReadBuffer(cmdQueue, (IMem)memoutput, Bool.True, IntPtr.Zero, new IntPtr(size * size * sizeof(int)), a, 0, null, out event0);

            clock.Stop();
            Console.WriteLine($"\nProgram in OpenCl completed in: {clock.Elapsed.TotalMilliseconds} miliseconds");

        }
    }
}
