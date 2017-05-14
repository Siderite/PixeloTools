using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixeloTools.Solving
{
    public class PersistentListEnumerator<T> : IEnumerator<T>
    {
        private List<T> _list;
        private int _index;

        public PersistentListEnumerator(List<T> list)
        {
            _list = list;
            _index = -1;
        }

        public int Index => _index;
        public int Count => _list.Count;

        public T Current
        {
            get
            {
                if (_index < 0) throw new Exception("Use MoveNext before you read a value");
                return _list[_index];
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }

        public bool MoveNext()
        {
            _index++;
            if (_index >= _list.Count) return false;
            return true;
        }

        public void Reset()
        {
            _index = -1;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _index = 0;
                    _list = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~PersistentListEnumerator() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
