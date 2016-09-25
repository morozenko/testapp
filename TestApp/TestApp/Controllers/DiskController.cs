using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;

namespace TestApp.Controllers
{
    public class DiskController : ApiController
    {
        private string[] drives;

        public DiskController ()
        {
            drives = System.Environment.GetLogicalDrives();
        }

        [ResponseType(typeof(string[]))]
        public async Task<IHttpActionResult> Get()
        {
            return this.Ok(drives);
        }

        [ResponseType(typeof(Dictionary<string, string[]>))]
        public async Task<IHttpActionResult> Get(string directory)
        {
            string[] dirs = GetDirectoryDirs(directory);
            string[] files = GetDirectoryFiles(directory);

            int[] sizes = GetSizes(directory);
            string[] size = {"0", "0", "0"};

            for (int i = 0; i < sizes.Length; i++)
            {
                size[i] = sizes[i].ToString();
            }
            

            var response = new Dictionary<string, string[]>();
            response["folders"] = dirs;
            response["files"] = files;
            response["sizes"] = size;

            return this.Ok(response);
        }

        protected string[] GetDirectoryDirs(string rootDirectory)
        {
            string[] subDirs = null;
            try
            {
                subDirs = System.IO.Directory.GetDirectories(rootDirectory);
            }
            catch (UnauthorizedAccessException e)
            {
                ;
            }
            catch (System.IO.DirectoryNotFoundException e)
            {
                ;
            }
            catch (SystemException)
            {
                ;
            }

            return subDirs;
        }

        protected string[] GetDirectoryFiles(string rootDirectory)
        {
            string[] files = null;

            try
            {
                files = System.IO.Directory.GetFiles(rootDirectory);
            }
            catch (UnauthorizedAccessException e)
            {
                ;
            }
            catch (System.IO.DirectoryNotFoundException e)
            {
                ;
            }
            catch (SystemException)
            {
                ;
            }

            return files;
        }

        // return array of int[3] where
        // int[0] amount of items <=10Mb
        // int[1] amount of items > 10mb AND <= 50mb
        // int[2] amount of items >= 100mb
        protected int[] GetSizes(string rootDir)
        {
            int MEGABYTE = 1024 * 1024;
            int[] sizes = { 0, 0, 0 };

            string[] subDirs = null;
            /*try
            {
                subDirs = GetDirectoryDirs(rootDir);

                if (null != subDirs)
                {
                    foreach (string dir in subDirs)
                    {
                        int[] subsize = GetSizes(dir);
                        sizes[0] += subsize[0];
                        sizes[1] += subsize[1];
                        sizes[2] += subsize[2];
                    }
                }
            }
            catch (UnauthorizedAccessException e)
            {
                ;
            }
            catch (System.IO.DirectoryNotFoundException e)
            {
                ;
            }
            catch (SystemException)
            {
                ;
            }*/

            string[] files = GetDirectoryFiles(rootDir);

            if (null != files)
            {
                foreach (string path in files)
                {
                    try
                    {
                        long length = new System.IO.FileInfo(path).Length;

                        if (10 * MEGABYTE >= length)
                        {
                            sizes[0]++;
                        }
                        else if ((10 * MEGABYTE < length) && (length < 50 * MEGABYTE))
                        {
                            sizes[1]++;
                        }
                        else if (100 * MEGABYTE <= length)
                        {
                            sizes[2]++;
                        }
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        ;
                    }
                    catch (System.IO.DirectoryNotFoundException e)
                    {
                        ;
                    }
                    catch (SystemException)
                    {
                        ;
                    }
                }
            }

            return sizes;
        }
    }
}
