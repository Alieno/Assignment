using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBT<T>
{
    protected List<T> m_data = new List<T>();
    protected int m_size;

    public CBT()
    {
        m_size = 0;
    }
    public int Parent(int index)
    {
        if (index == 0 || !Check(index))
        {
            return -1;
        }
        return (index - 1) / 2;
    }

    public int Left(int index)
    {
        if (!Check(index))
        {
            return -1;
        }
        int ret = /*(index + 1) * 2 - 1*/index * 2 + 1;
        return Check(ret) ? ret : -1;
    }

    public int Right(int index)
    {
        if (!Check(index))
        {
            return -1;
        }
        int ret = index * 2 + 2;
        return Check(ret) ? ret : -1;
    }

    public void Insert(T value)
    {
        m_data.Add(value);
        ++m_size;
    }
    public bool Check(int index)
    {
        return index >= 0 && index < m_size;
    }
};
public class PriorityQueue<KT, VT> : CBT<KeyValuePair<KT, VT>> where KT : struct
{
	//using ValueType = KeyValuePair<KT, VT>;
	//using PosMap = Dictionary<VT, KeyValuePair<KT, int>>;

	Dictionary<VT, KeyValuePair<KT, int>> m_pos = new Dictionary<VT, KeyValuePair<KT, int>>();
    ValueCompare m_comp = new ValueCompare();

    public PriorityQueue() : base()
	{
	}

    public PriorityQueue(IComparer comparer) : base()
    {
		m_comp = new ValueCompare(comparer);
    }

    public PriorityQueue(List<KeyValuePair<KT, VT>> A) : base()
	{
		m_data = A;
		m_size = m_data.Count;
		for (int i = Parent(m_size - 1); i >= 0; --i)
		{
			Heapify(i);
		}
	}

	virtual public void Insert(KT key, VT value)
	{
		base.Insert(new KeyValuePair<KT, VT>(key, value));
		int index = m_size - 1;
		m_pos[value] = new KeyValuePair<KT, int>(key, index);
		while (index > 0 && m_comp.Compare(m_data[index], m_data[Parent(index)]) > 0)
		{
			int p = Parent(index);
			ExchangePos(index, p);
			index = p;
		}
	}

	virtual public KeyValuePair<KT, VT> Extremum()
	{
        Debug.Assert(m_size > 0);
        return m_data[0];
	}

	virtual public KeyValuePair<KT, VT> Extract()
	{
		Debug.Assert(m_size > 0);
		KeyValuePair<KT, VT> ret = m_data[0];
		m_data[0] = m_data[m_data.Count - 1];
		m_data.RemoveAt(m_data.Count - 1);
		--m_size;
		m_pos.Remove(ret.Value);
		Heapify(0);
		return ret;
	}

	virtual public bool Promote(VT value, KT priority)
	{
		if (!m_pos.ContainsKey(value))
		{
			return false;
		}
		int index = m_pos[value].Value;
		if (m_comp.Compare(m_data[index], new KeyValuePair<KT, VT>(priority, value)) > 0)
		{
			return false;
		}
		m_pos[value] = new KeyValuePair<KT, int>(priority, index);
		m_data[index] = new KeyValuePair<KT, VT>(priority, m_data[index].Value);
		while (index > 0 && m_comp.Compare(m_data[index], m_data[Parent(index)]) > 0)
		{
			int p = Parent(index);
			ExchangePos(index, p);
			index = p;
		}
		return true;
	}

	public bool Empty()
	{
		return m_size == 0;
	}

	virtual protected void Heapify(int index)
	{
		if (!Check(index))
		{
			return;
		}
		int ex = index;
		int l = Left(index);
		int r = Right(index);
		if (l != -1 && m_comp.Compare(m_data[l], m_data[ex]) > 0)
		{
			ex = l;
		}
		if (r != -1 && m_comp.Compare(m_data[r], m_data[ex]) > 0)
		{
			ex = r;
		}
		if (ex == index)
		{
			return;
		}
		ExchangePos(index, ex);
		Heapify(ex);
	}

	protected void ExchangePos(int a, int b)
	{
		KeyValuePair<KT, int> value;
		if (m_pos.TryGetValue(m_data[a].Value, out value))
		{
			m_pos[m_data[a].Value] = new KeyValuePair<KT, int>(value.Key, b);
		}
		if (m_pos.TryGetValue(m_data[b].Value, out value))
		{
            m_pos[m_data[b].Value] = new KeyValuePair<KT, int>(value.Key, a);
        }
        Exchange(a, b);
	}
	protected void Exchange(int a, int b)
	{
		KeyValuePair<KT, VT> tmp = m_data[a];
		m_data[a] = m_data[b];
		m_data[b] = tmp;
	}

    private class ValueCompare
	{
        public ValueCompare()
        {
        }

        public ValueCompare(IComparer comparer)
		{
			m_comp = comparer;
		}

		public int Compare(KeyValuePair<KT, VT> left, KeyValuePair<KT, VT> right)
		{
			return m_comp.Compare(left.Key, right.Key);
		}

		protected IComparer m_comp = Comparer<KT>.Default;
	};
};
