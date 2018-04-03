using System;
using System.Collections.Generic;

public class PriorityQueue<T> {
	public delegate void DumpCB(T value);
    IComparer<T> comparer;
    T[] heap;
    public int Count { get; private set; }

    public PriorityQueue() : this(null) {
    }

    public PriorityQueue(int capacity) : this(capacity, null) {
    }

    public PriorityQueue(IComparer<T> comparer) : this(16, comparer) {
    }

    public PriorityQueue(int capacity, IComparer<T> comparer) {
        this.comparer = comparer == null ? Comparer<T>.Default : comparer;
        this.heap = new T[capacity];
    }

    public void Push(T v) {
        if (this.Count >= heap.Length)
            Array.Resize(ref heap, this.Count * 2);
        this.heap[this.Count] = v;
        SiftUp(this.Count++);
    }

    public T Pop() {
        var v = Top();
        this.heap[0] = this.heap[--this.Count];
        if (this.Count > 0) {
            SiftDown(0);
        }
        return v;
    }

    public T Top() {
        if (this.Count > 0) {
            return this.heap[0];
        }
        throw new InvalidOperationException("优先队列为空");
    }

    private void SiftUp(int n) {
        var v = this.heap[n];
        for (var n2 = n / 2; n > 0 && comparer.Compare(v, this.heap[n2]) > 0; n = n2, n2 /= 2) {
            this.heap[n] = this.heap[n2];
        }
        this.heap[n] = v;
    }

    private void SiftDown(int n) {
        var v = this.heap[n];
        for (var n2 = n * 2; n2 < this.Count; n = n2, n2 *= 2) {
            if (n2 + 1 < this.Count && comparer.Compare(this.heap[n2 + 1], this.heap[n2]) > 0)
                n2++;
            if (comparer.Compare(v, this.heap[n2]) >= 0)
                break;
            this.heap[n] = this.heap[n2];
        }
        this.heap[n] = v;
    }

    public void Clear() {
        Array.Clear(this.heap, 0, this.heap.Length);
        this.Count = 0;
    }

	public void Dump(DumpCB cb) {
		for (var i = 0; i < this.Count; i++) {
			cb(this.heap[i]);
		}
	}
}