#include "pch.h"
#include <iostream>
#include "mpi.h"
using namespace std;

#define SIZE 5000
#define FROM_MASTER 1
#define FROM_WORKER 2

MPI_Status status;

static double a[SIZE][SIZE];
static double b[SIZE][SIZE];
static double c[SIZE][SIZE];

static void random_matrix()
{
	for (int i = 0; i < SIZE; i++)
	{
		for (int j = 0; j < SIZE; j++)
		{
			a[i][j] = rand() % 100 + 1;
			b[i][j] = rand() % 100 + 1;
		} 
	}  
} 

int main(int argc, char **argv)
{
	int myrank, process_count, rows;
	int mtype;
	int offseta, offsetb;
	double start_time, end_time;
	MPI_Init(&argc, &argv);
	MPI_Comm_size(MPI_COMM_WORLD, &process_count);
	MPI_Comm_rank(MPI_COMM_WORLD, &myrank);

	rows = SIZE / process_count;  
	mtype = FROM_MASTER; 

	random_matrix();
	if (myrank == 0) 
	{
		printf("matrix size: %d \nnumber of process: %d\n", SIZE, process_count);
		
		start_time = MPI_Wtime();

		if (process_count == 1) 
		{
			for (int i = 0; i < SIZE; i++) 
			{
				for (int j = 0; j < SIZE; j++) 
				{
					c[i][j] = c[i][j] + a[i][j] - b[i][j];
				} 
			}  
			end_time = MPI_Wtime();
			printf("program of %2d process execute in: %f\n", process_count, end_time - start_time);
		}

		else 
		{

			for (int l = 0; l < process_count; l++) {
				offsetb = rows * l; 
				offseta = rows;
				mtype = FROM_MASTER;

				for (int dest = 1; dest < process_count; dest++)
				{
					MPI_Send(&offseta, 1, MPI_INT, dest, mtype,	MPI_COMM_WORLD);
					MPI_Send(&offsetb, 1, MPI_INT, dest, mtype,	MPI_COMM_WORLD);
					MPI_Send(&rows, 1, MPI_INT, dest, mtype, MPI_COMM_WORLD);
					MPI_Send(&a[offseta][0], rows*SIZE, MPI_DOUBLE, dest, mtype, MPI_COMM_WORLD);
					MPI_Send(&b[0][offsetb], rows*SIZE, MPI_DOUBLE, dest, mtype, MPI_COMM_WORLD);

					offseta += rows;
					offsetb = (offsetb + rows) % SIZE;

				} 

				offseta = rows;
				offsetb = rows * l;

				for (int i = 0; i < offseta; i++) 
				{
					for (int j = offsetb; j < offsetb + rows; j++) 
					{
						c[i][j] = c[i][j] + a[i][j] - b[i][j];
					}
				}
				   
				mtype = FROM_WORKER;
				for (int src = 1; src < process_count; src++) {
					MPI_Recv(&offseta, 1, MPI_INT, src, mtype, MPI_COMM_WORLD,	&status);
					MPI_Recv(&offsetb, 1, MPI_INT, src, mtype, MPI_COMM_WORLD,	&status);
					MPI_Recv(&rows, 1, MPI_INT, src, mtype, MPI_COMM_WORLD,		&status);
					for (int i = 0; i < rows; i++) 
					{
						MPI_Recv(&c[offseta + i][offsetb], offseta, MPI_DOUBLE, src, mtype, MPI_COMM_WORLD, &status);
					} 
				}
			} 
			end_time = MPI_Wtime();
			printf("program of %2d process execute in: %f\n", process_count, end_time - start_time);
		}
	} 

	else {
		
		if (process_count > 1) {
			start_time = MPI_Wtime();
			for (int l = 0; l < process_count; l++) {
				mtype = FROM_MASTER;
				MPI_Recv(&offseta, 1, MPI_INT, 0, mtype, MPI_COMM_WORLD,&status);
				MPI_Recv(&offsetb, 1, MPI_INT, 0, mtype, MPI_COMM_WORLD,&status);
				MPI_Recv(&rows, 1, MPI_INT, 0, mtype, MPI_COMM_WORLD,	&status);

				MPI_Recv(&a[offseta][0], rows*SIZE, MPI_DOUBLE, 0, mtype,MPI_COMM_WORLD, &status);
				MPI_Recv(&b[0][offsetb], rows*SIZE, MPI_DOUBLE, 0, mtype,MPI_COMM_WORLD, &status);

				for (int i = 0; i < offseta; i++)
				{
					for (int j = offsetb; j < offsetb + rows; j++)
					{
						c[i][j] = c[i][j] + a[i][j] - b[i][j];
					}
				}

				mtype = FROM_WORKER;
				MPI_Send(&offseta, 1, MPI_INT, 0, mtype, MPI_COMM_WORLD);
				MPI_Send(&offsetb, 1, MPI_INT, 0, mtype, MPI_COMM_WORLD);
				MPI_Send(&rows, 1, MPI_INT, 0, mtype, MPI_COMM_WORLD);
				for (int i = 0; i < rows; i++) {
					MPI_Send(&c[offseta + i][offsetb], offseta, MPI_DOUBLE, 0,
						mtype, MPI_COMM_WORLD);

				} 
			}
			end_time = MPI_Wtime();
			printf("program of %2d process execute in: %f\n", process_count, end_time - start_time);
		} 
	} 

	MPI_Finalize();
	return 0;
}
