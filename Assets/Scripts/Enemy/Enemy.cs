using System.Collections;
using UnityEngine;

public interface Enemy //적 오브젝트들이 상속해야 할 인터페이스.
{
    //적 오브젝트들이 데미지를 받기위한 함수.
    public void TakeDamage(int damage);
}
