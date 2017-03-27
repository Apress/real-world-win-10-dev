using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Windows.Storage
{
    public static class StorageFileExtensions
    {
        async public static Task AppendLinesAsync(this StorageFile file, IEnumerable<string> lines)
        {
            await FileIO.AppendLinesAsync(file, lines);
        }

        async public static Task AppendTextAsync(this StorageFile file, string contents)
        {
            await FileIO.AppendTextAsync(file, contents);
        }

        async public static Task<IBuffer> ReadBufferAsync(this StorageFile file)
        {
            return await FileIO.ReadBufferAsync(file);
        }

        async public static Task<IList<string>> ReadLinesAsync(this StorageFile file)
        {
            return await FileIO.ReadLinesAsync(file);
        }

        async public static Task<string> ReadTextAsync(this StorageFile file)
        {
            return await FileIO.ReadTextAsync(file);
        }

        async public static Task WriteBufferAsync(this StorageFile file, IBuffer buffer)
        {
            await FileIO.WriteBufferAsync(file, buffer);
        }
        async public static Task WriteBytesAsync(this StorageFile file, byte[] buffer)
        {
            await FileIO.WriteBytesAsync(file, buffer);
        }

        async public static Task WriteLinesAsync(this StorageFile file, IEnumerable<string> lines)
        {
            await FileIO.WriteLinesAsync(file, lines);
        }

        async public static Task WriteTextAsync(this StorageFile file, string contents)
        {
            await FileIO.WriteTextAsync(file, contents);
        }

    }
}
