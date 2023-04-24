import Image from 'next/image'
import { Inter } from 'next/font/google'
import Link from 'next/link'
import { useState } from 'react'

const inter = Inter({ subsets: ['latin'] })

export async function getServerSideProps(context:any) {
    const name = context.params.bondName;
    const res = await fetch(process.env.requesturl+name);
    const data = await res.json();

    return {
        props: {
            data,
        },
    };
}

export default function Home({ data}:{data:any}) {
const [query, setQuery] = useState('');

  const searchFilter = (array:any) => {
	  return array.filter(
	    (el:any) => el.price>(query)
	  )
  }

  const filtered = searchFilter(data)

const handleChange = (e:any) => {
setQuery(e.target.value);
}


  return (
    <main className="flex min-h-screen flex-col items-center justify-between p-24">
       <h1><Link href="/">            <span className="inline-block transition-transform group-hover:translate-x-1 motion-reduce:transform-none">
              &lt;-
            </span> {data[0]?.bondName}</Link></h1>
            <input onChange={(e)=>handleChange(e)} type='text' placeholder='Search...'/>
      <div className="mb-128 grid text-center lg:mb-12 lg:grid-cols-1 lg:text-left">
      
       {data?.Length === 0 ? (
        <h1>test</h1>
       ):(
         filtered.map((data:any) => (
			<div
          className="group rounded-lg border border-transparent px-5 py-4 transition-colors hover:border-gray-300 hover:bg-gray-100 hover:dark:border-neutral-700 hover:dark:bg-neutral-800/30"
          rel="noopener noreferrer"
          key = {data.timeStamp}
        >
          <p
            className="m-0 max-w-[30ch] text-sm opacity-50"
          >
            Price: {data.price.toFixed(3)} $
          </p>
          <p
            className="m-0 max-w-[30ch] text-sm opacity-50"
          >
            On Date: {data.timeStamp}
          </p>
        </div>
                ))
	)}
    
      
      </div>
    </main>
  )
}



