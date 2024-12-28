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

INotifyPropertyChanged 패턴을 따르는 클래스입니다.

T 타입의 값을 감싸고, 값이 변경될 때 자동으로 구독자에게 알림을 보내는 기능을 제공합니다

## 사용법
### 예제
```C#
var observableInt = new Observable<int>();

//구독
observableInt.PropertyChanged += (sender, args) =>
{
	if (sender is not Observable<int> observable)
		return;
	Console.WriteLine($"값 변경: {observable.Value}");
};

//값 변경시 구독 이벤트 발생
observableInt.Value = 10; //출력: "값 변경: 10"
```

### 암시적 변환
```c#
var observableInt = new Observable<int>();
observableInt.Value = 10;

int value = observableInt.Value;
```
암시적 변환을 사용하여 내부 값을 접근할 수 있습니다:

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
