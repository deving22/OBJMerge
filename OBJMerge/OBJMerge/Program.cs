using System;
using System.Collections.Generic;
using System.IO;

namespace OBJMerge
{
    class Program
    {
        static void Main(string[] args)
        {
            MergeOBJ newInstance = new MergeOBJ();
            

            //Import contents of first OBJ file (file to be kept)
            (List<string> v1Contents, List<string> i1Contents, int firstIndexCount) = newInstance.ImportFirstOBJ();

            int numVertices1 = v1Contents.Count;
            Console.WriteLine("Number of vertices from first obj: " + numVertices1);

            Console.WriteLine("Number of indices from first obj: " + firstIndexCount);

            //Console.WriteLine("Vertex info: " + "\n");
            //vContents.Add("Test Line to add"); 


            //Console.WriteLine("\n" + "Index info: " + "\n");
            //iContents.ForEach(Console.WriteLine);

            //Import contents of second OBJ file (file to be merged)
            //Console.WriteLine("\n" + "Second OBJ File: ");
            //Console.WriteLine("Vertex info: " + "\n");
            (List<string> v2Contents, List<string> i2Contents, int secondIndexCount) = newInstance.ImportSecondOBJ();
            //v2Contents.ForEach(Console.WriteLine);
            //Console.WriteLine("Index info: " + "\n");
            //Console.WriteLine("Index Count for first one: " + i1Contents.Count);

            int numVertices2 = v2Contents.Count;
            Console.WriteLine("Number of vertices from second obj: " + numVertices2);

            int numIndices2 = i2Contents.Count;
            Console.WriteLine("Number of indices from second obj: " + secondIndexCount);

            //v2Contents.Add("Test Line to add");
            //i1Contents.ForEach(Console.WriteLine);
            i2Contents.ForEach(Console.WriteLine);

            List<int> secondOBJIndexNumbers = newInstance.ModifyIDXSection(i2Contents, numVertices1);

            (List<string> newi2Contents, int totalSecondIndices) = newInstance.ReformatNewIDXSection(secondOBJIndexNumbers);
            //secondOBJIndexNumbers.ForEach(Console.WriteLine);

            //For editing TRIS
            //Console.WriteLine("First OBJ Number at Index 16878: " + i1Contents[16878]);
            Console.WriteLine("Second OBJ Number at Index 1: " + secondOBJIndexNumbers[1]);

            List<string> animContents = newInstance.GetANIM();

            //Output statements
            Console.WriteLine("New OBJ File: " + "\n");
            v1Contents.ForEach(Console.WriteLine);
            v2Contents.ForEach(Console.WriteLine);
            Console.WriteLine("\n");
            i1Contents.ForEach(Console.WriteLine);
            Console.WriteLine("BREAK");
            newi2Contents.ForEach(Console.WriteLine);

            //Compute new header information
            int newTotalTriangles = numVertices1 + numVertices2;
            int newTotalIndices = firstIndexCount + secondIndexCount;
            Console.WriteLine("Use this number for next TRIS animation: " + firstIndexCount);

            /* Write everything to a new file */

            //System.IO.File.WriteAllLines(@"C:\Users\Devin Grande\Documents\OBJMerge\output.obj", v1Contents);

            //Start new file and add all header info
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"C:\Users\Devin Grande\Documents\OBJMerge\output.obj"))
            {
                //file.WriteLine("A");
                //file.WriteLine("800");
                //file.WriteLine("OBJ");
                //file.WriteLine("\n");
                //file.WriteLine("GLOBAL_specular 1.0");
                //file.WriteLine("GLOBAL_cockpit_lit");
                //file.WriteLine("\n");
                file.WriteLine("POINT_COUNTS " + newTotalTriangles + " 0 0 " + newTotalIndices);
            }

            //Append contents (V1,V2,I1,new I2)
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"C:\Users\Devin Grande\Documents\OBJMerge\output.obj",true))
            {
                foreach (string line in v1Contents)
                {
                    file.WriteLine(line);
                }

                file.WriteLine("\n");

                foreach (string line in v2Contents)
                {
                    file.WriteLine(line);
                }

                file.WriteLine("\n");

                foreach (string line in i1Contents)
                {
                    file.WriteLine(line);
                }

                file.WriteLine("\n");

                foreach (string line in newi2Contents)
                {
                    file.WriteLine(line);
                }

                file.WriteLine("\n");

                foreach (string line in animContents)
                {
                    //file.WriteLine(line);
                }
            }
        }
    }

    class MergeOBJ
    {
        public (List<string>, List<string>, int) ImportFirstOBJ()
        {
            //Initialize counters for vertices, indices
            int vertexCounter = 0;
            int indexCounter = 0;

            //Initialize lists for vertex content, index content
            List<string> vertexContents = new List<string>();
            List<string> indexContents = new List<string>();

            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(@"C:\Users\Devin Grande\Documents\OBJMerge\Cessna_172SP_cockpit.obj"))
                {
                    string line;
                    
                    // Read and display lines from the file until the end of
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        //Find number of vertices in file
                        
                        if (line.Contains("VT"))
                        {
                            vertexCounter++;

                            vertexContents.Add(line);
                        }

                        if (line.Contains("IDX"))
                        {
                            if(line.Contains("IDX10"))
                            {
                                indexCounter+=10;
                            }
                            else
                            {
                                indexCounter++;
                            }

                            indexContents.Add(line);
                        }
                        
                    }
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            return (vertexContents, indexContents, indexCounter);
        }

        //Get total number of indices (same as number of vertices)
        //Import second obj vertex info, save to "appendedContents" variable
        //Get total number of lines of appended Contents
        //Append vertex info to end of vertex block from first obj
        //For index lines, step through character by character and add number of existing vertices to this number. Should update index position

        //For animations area, add number of existing triangles to first number after TRIS. Second number stays the same
        //Output new obj file

        public (List<string>,List<string>, int) ImportSecondOBJ()
        {
            //Initialize lists for vertex content, index content
            List<string> vertexContents = new List<string>();
            List<string> indexContents = new List<string>();
            
            //Initialize index counter
            int indexCounter = 0;

            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(@"C:\Users\Devin Grande\Documents\OBJMerge\cockpit_clickspots_new_anim.obj"))
                {
                    string line;

                    // Read and display lines from the file until the end of
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        //Find number of vertices in file

                        if (line.Contains("VT"))
                        {
                            vertexContents.Add(line);
                        }

                        if (line.Contains("IDX"))
                        {
                            if (line.Contains("IDX10"))
                            {
                                indexCounter += 10;
                            }
                            else
                            {
                                indexCounter++;
                            }

                            indexContents.Add(line);
                        }

                    }
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            
            return (vertexContents,indexContents, indexCounter);
        }

        public List<int> ModifyIDXSection(List<string> indexContents, int runningTotal)
        {
            //Split string into new list with each entry being a number (first entry will be IDX or IDX10)
            //Console.WriteLine("About to start writing lines: ");
            int startIndex = runningTotal;

            string[] catchContents;
            
            List<int> numbers = new List<int>();

            List<int> newNumbers = new List<int>();

            foreach (string line in indexContents)
            {
                catchContents = line.Split('\t');

                foreach (string entry in catchContents)
                {
                    if (entry != "IDX" && entry != "IDX10")
                    {
                        //Console.WriteLine("Non IDX/IDX10 Entry: " + entry);
                        numbers.Add(Int32.Parse(entry));
                    }
                    
                }
            }
            foreach (int num in numbers)
            {
                newNumbers.Add(num + startIndex);
            }

            return newNumbers;
        }

        public (List<string>, int) ReformatNewIDXSection(List<int> indexNumbers)
        {
            int[] numberArray;
            List<string> newi2Contents = new List<string>();

            int sum = indexNumbers.Count;

            int numberIDX10Rows = sum / 10;
            int numberIDXRows = sum % 10;
            //Console.WriteLine("Number of IDX 10 rows will be: " + numberIDX10Rows);
            //Console.WriteLine("Number of IDX rows will be: " + numberIDXRows);
            int totalSecondIndices = numberIDX10Rows * 10 + numberIDXRows;
            Console.WriteLine("POINT_COUNT Indices: " + totalSecondIndices);

            //Write contents of IDX10 section
            for (int i = 0; i < (sum-numberIDXRows) ; i += 10)
            {
                numberArray = indexNumbers.GetRange((i + 0), 10).ToArray();
                //Console.WriteLine("IDX10 " + "{0}", string.Join(" ", numberArray));

                newi2Contents.Add("IDX10 " + string.Join(" ", numberArray));
            }

            //Write contents of IDX section
            for (int i = (numberIDX10Rows * 10); i < (sum); i++)
            {
                numberArray = indexNumbers.GetRange((i + 0), 1).ToArray();
                //Console.WriteLine("IDX " + "{0}", string.Join(" ", numberArray));
                newi2Contents.Add("IDX " + string.Join(" ", numberArray));
            }

            return (newi2Contents, totalSecondIndices);
        }

        public List<string> GetANIM()
        {
            List<string> animContents = new List<string>();
            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(@"C:\Users\Devin Grande\Documents\OBJMerge\Cessna_172SP_cockpit.obj"))
                {
                    string line;

                    // Read and display lines from the file until the end of
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        //Find number of vertices in file

                        if (line.Contains("ANIM") || line.Contains("TRIS") || line.Contains("ATTR") || line.Contains("MAGNET"))
                        {
                            animContents.Add(line);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            return animContents;
        }
    }
}
        
