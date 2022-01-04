using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
//using System.Collections;

namespace SaveTextFile
{
    class FileProcessing
    {
       private string path_file ;
       private string path_file1;
       private float[] coefficient;
       private float[] coefficient1;
      public int n; //so luong mau muon hien thi
       private int m;
          //Ham khoi tao
       public FileProcessing(string name)
       {
           path_file = name;
           path_file1 = name;
           
       }
        ////////////////////////
        //Ham doc file
        private void FileRead ()
        {
            StreamReader myreader = new StreamReader(path_file);
            coefficient = new float[n];
            for (int i = 0 ; i< n ; i ++)
            {
                string s = myreader.ReadLine();
                coefficient[i] = (float)(Convert.ChangeType(s,typeof(float)));

            }
            myreader.Close();  //dong file
            myreader.Dispose();//realise  
        }
        private void FileRead1()
        {
            StreamReader myreader1 = new StreamReader(path_file1);
            coefficient1 = new float[m];
            for (int i = 0; i < m; i++)
            {
                string s = myreader1.ReadLine();
                coefficient1[i] = (float)(Convert.ChangeType(s, typeof(float)));

            }
            myreader1.Close();  //dong file
            myreader1.Dispose();//realise 
        }
        public void Data_Coefficient (out float [] output)
        {
            output = new float[n];
            FileRead();

            for (int i = 0 ; i < n ; i ++)
            {
                output[i] = coefficient[i];
            }
        }
        public void Data_Coefficient1(out float[] output1)
        {
            output1 = new float[m];
            FileRead1();

            for (int j = 0; j < m; j++)
            {
                output1[j] = coefficient1[j];
            }
        }
        /***************************/
        //Ham luu data vao file
        public void SaveData(int[] data_save)
        {
            if (File.Exists(path_file) == true)  //kiem tra xem file co ton tai khong
            {
                File.Delete(path_file); //xoa file cu di
            }
            StreamWriter my_write = new StreamWriter(path_file, true);
            for (int i = 0; i < data_save.Length; i++)
            {
                my_write.WriteLine(data_save[i].ToString());
            }
           // my_write.WriteLine("Length : {0}", data_save.Length);
            my_write.Close();
            my_write.Dispose();
        }

        public void SaveData1(int[] data_save1)
        {
            if (File.Exists(path_file1) == true)  //kiem tra xem file co ton tai khong
            {
                File.Delete(path_file1); //xoa file cu di
            }
            StreamWriter my_write1 = new StreamWriter(path_file1, true);
            for (int j = 0; j < data_save1.Length; j++)
            {
                my_write1.WriteLine(data_save1[j].ToString());
            }
         //  my_write1.WriteLine("Length : {0}", data_save1.Length);
            my_write1.Close();
            my_write1.Dispose();
        }
    }
}


