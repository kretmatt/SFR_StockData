import Image from 'next/image'
import { Inter } from 'next/font/google'
import Link from 'next/link'
const inter = Inter({ subsets: ['latin'] })


export async function getServerSideProps(context) {
    const name = context.params.bondName;
    const res = await fetch('http://localhost:8087/Stock/'+name);
    const data = await res.json();

    return {
        props: {
            data,
        },
    };
}

export default function Home({ data}) {
  return (
    <main className="flex min-h-screen flex-col items-center justify-between p-24">
       <h1><Link href="/">{data[0]?.bondName}</Link></h1>
      <div className="mb-32 grid text-center lg:mb-0 lg:grid-cols-1 lg:text-left">
      
       {data?.Length === 0 ? (
        <h1>test</h1>
       ):(
         data.map((data) => (
			<div
          className="group rounded-lg border border-transparent px-5 py-4 transition-colors hover:border-gray-300 hover:bg-gray-100 hover:dark:border-neutral-700 hover:dark:bg-neutral-800/30"
          target="_blank"
          rel="noopener noreferrer"
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



