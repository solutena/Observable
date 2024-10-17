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

단일 값(예: int, string, bool)의 변화를 감지할 수 있게 하는 객체입니다.

이 객체는 주로 데이터가 변경될 때 해당 변화를 구독자들에게 알리는 데 사용됩니다.

## 이벤트
- **OnChanged:** 값이 변경될 때마다 호출됩니다. 이를 통해 값이 변경될 때 특정 동작을 수행할 수 있습니다.

## 사용법

직렬화 및 역직렬화 시 데이터를 적절히 처리하는 방법을 보여주는 예제입니다.

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
### 이벤트 구독

```csharp
Hp.OnChanged += (int value) =>
{
  Debug.Log($"값 변경됨: {value}");
};
```

# ObservableCollection<T>

여러 개의 값이 있는 컬렉션(List<T>, Dictionary<K, V> 등)의 변경 사항을 감지할 수 있도록 설계된 컬렉션입니다.

## 함수
- **Add:** 항목을 컬렉션에 추가하고 `OnAdd` 이벤트를 발생시킵니다. 후에 `OnChanged` 이벤트를 발생시킵니다.
- **Remove:** 항목을 컬렉션에서 제거하고 `OnRemove` 이벤트를 발생시킵니다. 후에 `OnChanged` 이벤트를 발생시킵니다.
- **Clear:** 모든 항목을 삭제하고 `OnRemove` 이벤트를 개별 항목에 대해 발생시킵니다. 후에 `OnChanged` 이벤트를 발생시킵니다.
- **Refresh:** 컬렉션을 다시 알림 상태로 만들어 강제로 `OnChanged` 이벤트를 발생시킵니다.

## 이벤트
- **OnAdd:** 항목이 추가될 때 발생하는 이벤트입니다. 추가된 항목을 매개변수로 전달합니다.
- **OnRemove:** 항목이 삭제될 때 발생하는 이벤트입니다. 삭제된 항목을 매개변수로 전달합니다.
- **OnChanged:** 컬렉션에 변경이 생길 때마다 호출되는 이벤트입니다. 전체 컬렉션을 매개변수로 전달합니다.

## 사용법

`ObservableCollection<T>` 클래스는 `ICollection<T>`나 `IEnumerable<T>`로 초기화할 수 있습니다.

```csharp
List<int> list = new();
HashSet<int> hashset = new();
Dictionary<int, int> dictionary = new();

var observableList = new ObservableCollection<int>(list);
var observableHashSet = new ObservableCollection<int>(hashset);
var observableDictionary = new ObservableCollection<KeyValuePair<int, int>>(dictionary);
```

직렬화 및 역직렬화 시 데이터를 적절히 처리하는 방법을 보여주는 예제입니다.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Example : ISerializationCallbackReceiver
{
	[SerializeField] List<int> list;

	public ObservableCollection<int> List { get; set; }

	public void OnAfterDeserialize()
	{
		list = List.ToList();
	}

	public void OnBeforeSerialize()
	{
		List = new ObservableCollection<int>(list);
	}
}
```

### 이벤트 구독
```csharp
List.OnAdd += item => Debug.Log($"항목 추가됨: {item}");
List.OnRemove += item => Debug.Log($"항목 제거됨: {item}");
List.OnChanged += collection => Debug.Log("컬렉션이 변경되었습니다.");
```
