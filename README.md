# Observable

옵저버 패턴을 구현하여 값이 변경될 때 구독자들에게 알림을 보내는 기능을 제공합니다.

# 설치

![image](https://github.com/user-attachments/assets/174185db-6090-42e7-93b7-01b3f0701315)

1. URL 복사

![image](https://github.com/user-attachments/assets/f4060f1d-94aa-4a49-b001-e7a5e01316e1)

2. 패키지 매니저에서 Add Package from Git URL 선택

![image](https://github.com/user-attachments/assets/a4af4faf-2741-48ea-b525-29bc0a09688b)
   
3.  복사한 URL로 설치

# Observable\<T\>

`Observable<T>` 클래스는 T 타입의 값에서 변경 사항을 추적할 수 있는 간단한 방법을 제공합니다.

값이 업데이트될 때 이벤트를 발생시키는 메커니즘이 포함되어 있습니다.

## 이벤트
값이 변경되면 `OnChanged` 이벤트가 호출됩니다.

이 이벤트는 이전 값과 현재 값을 전달합니다.

## 암시적 변환
Observable<T> 객체를 T 타입으로 암시적으로 변환합니다.
```C#
var o = new Observable<int>(10);
int i = o;
```

## 사용법
### 예제
```C#
var observable = new Observable<int>(10);

//구독
observable.OnChanged += (prev, current) =>
{
    Console.WriteLine($"Value changed from {prev} to {current}");
};

//구독된 이벤트 실행
observable.Value = 20; // 출력: Value changed from 10 to 20
```

### 직렬화 데이터
```csharp
using System;
using UnityEngine;

[Serializable]
public class Example : ISerializationCallbackReceiver
{
	[SerializeField] int hp;

	public Observable<int> HP { get; set; }

	public void OnAfterDeserialize()
	{
		hp = HP;
	}

	public void OnBeforeSerialize()
	{
		HP = new Observable<int>(hp);
	}
}
```
직렬화 데이터에서 사용하는 예제입니다.

# 컬렉션
`ObservableDictionary<TKey, TValue>`

`ObservableHashSet<T>`

`ObservableList<T>`

지원되는 컬렉션입니다.

## 이벤트
컬렉션이 변경되면 두가지 이벤트가 호출됩니다.

`OnItemChanged` : 변경된 값과, 변경 타입을 전달합니다.

`OnCollectionChanged` : 변경된 컬렉션을 전달합니다.

## 변경 타입
`Added` : 추가

`Removed` : 제거

`Updated` : 변경
