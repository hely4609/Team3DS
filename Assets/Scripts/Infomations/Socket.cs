using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Socket : MonoBehaviour
{
    //소켓을 가지고 있을 컨테이너입니다!
    public class Container
    {
        //모든 내용을 찾는 함수를 저장할 수 있는 델리게이트입니다!
        delegate void FindAllDelegate(int hash, ref List<Socket> result);
        FindAllDelegate FindAll;
        //하나의 소켓을 찾는 함수를 저장할 수 있는 델리게이트입니다!
        delegate void FindDelegate(int hash, ref Socket result);
        FindDelegate Find;

        // 이 컨테이너를 가지고 있는 오브젝트를 찾을 수 있는 방법을 제공할 수 있도록 델리게이트를 열어둡니다!
        public Func<GameObject> GetOwner;
        //##선택## 이 컨테이너를 가지고 있는 캐릭터를 찾을 수 있는 방법을 제공합시다!
        // public Func<Character> GetCharacter;

        //소켓을 붙이기 위해 함수를 등록합시다!
        public void AttachSocket(Socket target)
        {
            //컨테이너를 나로 등록!
            target.myContainer = this;
            FindAll -= target.FindAllSocket;
            FindAll += target.FindAllSocket;
            Find -= target.FindSocket;
            Find += target.FindSocket;
        }

        //소켓을 해제합시다!
        public void DettachSocket(Socket target)
        {
            //컨테이너 해제를 하는 것이기 때문에, 대상의 컨테이너를 비워줍시다!
            if (target.myContainer == this) target.myContainer = null;
            FindAll -= target.FindAllSocket;
            Find -= target.FindSocket;
        }

        //이름을 주면 해당하는 소켓을 찾아봅시다!
        public Socket FindSocket(string name)
        {
            Socket result = null;
            Find?.Invoke(name.GetHashCode(), ref result); //해시 코드로 찾아서 내보내주시면 됩니다!
            return result;
        }

        //이름을 주면 모든 소켓을 찾아주도록 하겠습니다!
        public List<Socket> FindAllSocket(string name)
        {
            List<Socket> result = new(); //결과물이 될 리스트입니다!
            FindAll?.Invoke(name.GetHashCode(), ref result); //연결된 소켓들의 찾기 함수를 모두 돌리고
            return result; //결과물을 반환합시다!
        }
        
    }


    //날 가지고 있는 컨테이너를 확인합시다!
    public Container myContainer;

    //위치를 표시할 곳에 넣어둘 스크립트입니다!
    //예를 들어, 손이나 총구, 그립 포인트 등에 넣어둘 소켓 스크립트예요!
    //그래서 이 위치가 어디인지 이름을 넣을 필요가 있어요!
    [SerializeField] string _socketName;
    //소켓을 검색할 때 string대신 int로 검색할 수 있도록 도와줍시다!
    int socketHash;

    //이름을 다른 데에서 확인하게 하기 위해 프로퍼티로 열어둘게요!
    public string SocketName
    {
        get => _socketName;
        set
        {
            _socketName = value; //바꾸고 싶으면 바꾸는 거죠!
            socketHash = _socketName.GetHashCode(); //검색 속도를 위해 해시코드를 저장하도록 합시다!
        }
    }

    void Awake()
    {
        SocketName = _socketName; //처음에 에디터에서 설정해준 값으로 hashCode도 설정하기 위해 프로퍼티를 호출합니다!
    }

    //소켓의 주인을 찾기 위해         컨테이너에서 주인을 찾아줍시다!
    public GameObject GetOwner() => myContainer?.GetOwner();
    //public GameObject GetOwner() => gameObject;
    //소켓의 캐릭터를 찾기 위해         컨테이너에서 캐릭터를 찾아줍시다!
    //public Character GetCharacter() => myContainer?.GetCharacter();

    //모든 소켓을 찾는 함수입니다!
    void FindAllSocket(int hash, ref List<Socket> result)
    {
        //찾는 해시가 나의 해시랑 똑같으면 찾던 소켓이 내가 맞다고 추가해줍니다!
        if (hash == socketHash) result.Add(this);
    }

    //소켓 하나만 찾는 함수입니다!
    void FindSocket(int hash, ref Socket result)
    {
        //모든 소켓을 찾을 때와 마찬가지지만, 첫 번째로 찾은 것만 내보냅니다!
        if (result == null && hash == socketHash) result = this;
    }

    
}
