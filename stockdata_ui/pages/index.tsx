import Image from 'next/image'
import { Inter } from 'next/font/google'
import Link from 'next/link'
import getConfig from 'next/config'

const inter = Inter({ subsets: ['latin'] })

export async function getServerSideProps() {
    const res = await fetch(process.env.requesturl+'trends');
    const data = await res.json();

    return {
        props: {
            data,
        },
    };
}

export default function Home({ data }:{data:any}) {
  const urlBuilder = (stock:any) =>{
  	return "/stock/"+stock.bondName;
  }

  return (
    <main className="flex min-h-screen flex-col items-center justify-between p-24">
       <h1>Stocks Data</h1>
      <div className="mb-32 grid text-center lg:mb-0 lg:grid-cols-4 lg:text-left">
      
       {data?.Length === 0 ? (
        <h1>Still loading</h1>
       ):(
         data?.map((data:any) => (
         <Link key={data.bondName} href={urlBuilder(data)}>
			<div
          className="group rounded-lg border border-transparent px-5 py-4 transition-colors hover:border-gray-300 hover:bg-gray-100 hover:dark:border-neutral-700 hover:dark:bg-neutral-800/30"
          rel="noopener noreferrer"
        >
          <h2 className="mb-3 text-2xl font-semibold">
            {data.bondName}
          </h2>
          <p
            className="m-0 max-w-[30ch] text-sm opacity-50"
          >
            Overall change: {data.overallChange.toFixed(3)} %
          </p>
          <p
            className="m-0 max-w-[30ch] text-sm opacity-50"
          >
            Last minute change: {data.lastMinuteChange.toFixed(3)} %
          </p>
          <p
            className="m-0 max-w-[30ch] text-sm opacity-50"
          >
            Last hour change: {data.lastHourChange.toFixed(3)} %
          </p>
        </div></Link>
                ))
	)}
    
      
      </div>
    </main>
  )
}



