using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Socket : MonoBehaviour
{
    //������ ������ ���� �����̳��Դϴ�!
    public class Container
    {
        //��� ������ ã�� �Լ��� ������ �� �ִ� ��������Ʈ�Դϴ�!
        delegate void FindAllDelegate(int hash, ref List<Socket> result);
        FindAllDelegate FindAll;
        //�ϳ��� ������ ã�� �Լ��� ������ �� �ִ� ��������Ʈ�Դϴ�!
        delegate void FindDelegate(int hash, ref Socket result);
        FindDelegate Find;

        // �� �����̳ʸ� ������ �ִ� ������Ʈ�� ã�� �� �ִ� ����� ������ �� �ֵ��� ��������Ʈ�� ����Ӵϴ�!
        public Func<GameObject> GetOwner;
        //##����## �� �����̳ʸ� ������ �ִ� ĳ���͸� ã�� �� �ִ� ����� �����սô�!
        // public Func<Character> GetCharacter;

        //������ ���̱� ���� �Լ��� ����սô�!
        public void AttachSocket(Socket target)
        {
            //�����̳ʸ� ���� ���!
            target.myContainer = this;
            FindAll -= target.FindAllSocket;
            FindAll += target.FindAllSocket;
            Find -= target.FindSocket;
            Find += target.FindSocket;
        }

        //������ �����սô�!
        public void DettachSocket(Socket target)
        {
            //�����̳� ������ �ϴ� ���̱� ������, ����� �����̳ʸ� ����ݽô�!
            if (target.myContainer == this) target.myContainer = null;
            FindAll -= target.FindAllSocket;
            Find -= target.FindSocket;
        }

        //�̸��� �ָ� �ش��ϴ� ������ ã�ƺ��ô�!
        public Socket FindSocket(string name)
        {
            Socket result = null;
            Find?.Invoke(name.GetHashCode(), ref result); //�ؽ� �ڵ�� ã�Ƽ� �������ֽø� �˴ϴ�!
            return result;
        }

        //�̸��� �ָ� ��� ������ ã���ֵ��� �ϰڽ��ϴ�!
        public List<Socket> FindAllSocket(string name)
        {
            List<Socket> result = new(); //������� �� ����Ʈ�Դϴ�!
            FindAll?.Invoke(name.GetHashCode(), ref result); //����� ���ϵ��� ã�� �Լ��� ��� ������
            return result; //������� ��ȯ�սô�!
        }
        
    }


    //�� ������ �ִ� �����̳ʸ� Ȯ���սô�!
    public Container myContainer;

    //��ġ�� ǥ���� ���� �־�� ��ũ��Ʈ�Դϴ�!
    //���� ���, ���̳� �ѱ�, �׸� ����Ʈ � �־�� ���� ��ũ��Ʈ����!
    //�׷��� �� ��ġ�� ������� �̸��� ���� �ʿ䰡 �־��!
    [SerializeField] string _socketName;
    //������ �˻��� �� string��� int�� �˻��� �� �ֵ��� �����ݽô�!
    int socketHash;

    //�̸��� �ٸ� ������ Ȯ���ϰ� �ϱ� ���� ������Ƽ�� ����ѰԿ�!
    public string SocketName
    {
        get => _socketName;
        set
        {
            _socketName = value; //�ٲٰ� ������ �ٲٴ� ����!
            socketHash = _socketName.GetHashCode(); //�˻� �ӵ��� ���� �ؽ��ڵ带 �����ϵ��� �սô�!
        }
    }

    void Awake()
    {
        SocketName = _socketName; //ó���� �����Ϳ��� �������� ������ hashCode�� �����ϱ� ���� ������Ƽ�� ȣ���մϴ�!
    }

    //������ ������ ã�� ����         �����̳ʿ��� ������ ã���ݽô�!
    public GameObject GetOwner() => myContainer?.GetOwner();
    //public GameObject GetOwner() => gameObject;
    //������ ĳ���͸� ã�� ����         �����̳ʿ��� ĳ���͸� ã���ݽô�!
    //public Character GetCharacter() => myContainer?.GetCharacter();

    //��� ������ ã�� �Լ��Դϴ�!
    void FindAllSocket(int hash, ref List<Socket> result)
    {
        //ã�� �ؽð� ���� �ؽö� �Ȱ����� ã�� ������ ���� �´ٰ� �߰����ݴϴ�!
        if (hash == socketHash) result.Add(this);
    }

    //���� �ϳ��� ã�� �Լ��Դϴ�!
    void FindSocket(int hash, ref Socket result)
    {
        //��� ������ ã�� ���� ������������, ù ��°�� ã�� �͸� �������ϴ�!
        if (result == null && hash == socketHash) result = this;
    }

    
}
